import bpy, json, math
from pathlib import Path
BASE=Path(r'E:\Github-Fork\Open-Waters-repo\Assets\Models')
scene=bpy.data.scenes['OW_AssetWorkshop'];bpy.context.window.scene=scene;scene.frame_set(1)
manifest=[]
for coll in list(scene.collection.children):
 name=coll.name[3:];group=coll['export_group'];out=BASE/group;out.mkdir(exist_ok=True)
 source=out/'Sources~';source.mkdir(exist_ok=True)
 # Per-asset native source has a self-contained scene and only this collection.
 bpy.data.libraries.write(str(source/(name+'.blend')),{coll},path_remap='RELATIVE',fake_user=True,compress=True)
 bpy.ops.object.select_all(action='DESELECT')
 for o in coll.objects:o.select_set(True)
 bpy.context.view_layer.objects.active=next(o for o in coll.objects if o.parent is None)
 animated=bool(coll['animated'])
 # Ocean FBX stays flat: Unity shader owns displacement, avoiding doubled deformation.
 ocean_keys=None
 if name=='OceanSurface':
  o=next(o for o in coll.objects if o.type=='MESH');ocean_keys=o.data.shape_keys
  # Export a temporary flat mesh without shape keys. Source remains animated.
  temp=o.copy();temp.data=o.data.copy();coll.objects.link(temp);temp.shape_key_clear()
  for v in temp.data.vertices:v.co.z=0
  o.select_set(False);temp.select_set(True);animated=False
 bpy.ops.export_scene.fbx(filepath=str(out/(name+'.fbx')),use_selection=True,object_types={'MESH','EMPTY'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',use_mesh_modifiers=True,add_leaf_bones=False,bake_anim=animated,bake_anim_use_all_actions=False,bake_anim_use_nla_strips=False,bake_anim_force_startend_keying=True,bake_anim_simplify_factor=0.0,path_mode='RELATIVE',embed_textures=False)
 if ocean_keys:bpy.data.objects.remove(temp,do_unlink=True)
 tris=0;verts=0;bounds=[]
 for o in coll.objects:
  if o.type=='MESH':
   o.data.calc_loop_triangles();tris+=len(o.data.loop_triangles);verts+=len(o.data.vertices)
   for v in o.bound_box:bounds.append(list(o.matrix_world @ __import__('mathutils').Vector(v)))
 dims=[round(max(v[i] for v in bounds)-min(v[i] for v in bounds),3) for i in range(3)]
 manifest.append({'name':name,'group':group,'fbx':f'{group}/{name}.fbx','source':f'{group}/Sources~/{name}.blend','triangles':tris,'vertices':verts,'dimensionsBlenderXYZ':dims,'animatedFBX':animated,'sourceAnimated':bool(coll['animated']),'frames':[1,121] if animated else None,'fps':30,'unityAnimation':'shader' if name=='OceanSurface' else ('baked transform / blendshape clip' if animated else 'static')})
(BASE/'asset-manifest.json').write_text(json.dumps({'schema':1,'seed':2701,'units':'metres','sourceUp':'Z','sourceShipForward':'-Y','assets':manifest},indent=2))
print('Exported',len(manifest),'FBX files and native sources; total triangles',sum(a['triangles'] for a in manifest))
bpy.context.window.scene=bpy.data.scenes['OW_Harbor_Preview']

