import bpy,math,json
from mathutils import Vector
from pathlib import Path
BASE=Path(r'E:\Github-Fork\Open-Waters-repo\Assets\Models')
scene=bpy.data.scenes['OW_AssetWorkshop'];bpy.context.window.scene=scene
mat=bpy.data.materials['OW_Water'];nt=mat.node_tree
mapping=next(n for n in nt.nodes if n.type=='MAPPING');mapping.inputs['Scale'].default_value=(3,3,1)
p=next(n for n in nt.nodes if n.type=='BSDF_PRINCIPLED');p.inputs['Roughness'].default_value=.42;p.inputs['Specular IOR Level'].default_value=.18
# Fragmented polygon-following wash, independently placeable around CoastTile.
coll=bpy.data.collections.new('OW_ShoreFoam');scene.collection.children.link(coll);coll['export_group']='Water';coll['animated']=True
root=bpy.data.objects.new('ShoreFoam',None);coll.objects.link(root)
verts=[];faces=[]
for i in range(144):
 if i%12 in [8,9,10]:continue
 a=i*math.tau/144;b=(i+1)*math.tau/144
 # Radius of regular 12-gon on the side intersected by this ray.
 def edge(angle):return 5*math.cos(math.pi/12)/math.cos((angle%(math.tau/12))-math.pi/12)
 ra=edge(a)+.10;rb=edge(b)+.10;w=.025+.025*(.5+.5*math.sin(i*.5))
 k=len(verts);verts.extend([(ra*math.cos(a),ra*math.sin(a),0),(rb*math.cos(b),rb*math.sin(b),0),((rb+w)*math.cos(b),(rb+w)*math.sin(b),0),((ra+w)*math.cos(a),(ra+w)*math.sin(a),0)]);faces.append((k,k+1,k+2,k+3))
me=bpy.data.meshes.new('Shoreline wash');me.from_pydata(verts,[],faces);me.update();o=bpy.data.objects.new('Shoreline wash',me);coll.objects.link(o);o.parent=root
me.materials.append(bpy.data.materials['OW_HarborPalette']);uv=me.uv_layers.new(name='PaletteUV')
sample=next(o for o in bpy.data.collections['OW_FX_Wake'].objects if o.type=='MESH').data.uv_layers[0].data[0].uv.copy()
for v in uv.data:v.uv=sample
for f,s in [(1,1),(61,1.025),(121,1)]:root.scale=(s,s,1);root.keyframe_insert(data_path='scale',frame=f)
root.animation_data.action.name='ShoreFoam_TidalWash';scene.frame_set(1)
out=BASE/'Water';bpy.data.libraries.write(str(out/'Sources~'/'ShoreFoam.blend'),{coll},path_remap='RELATIVE',fake_user=True,compress=True)
bpy.ops.object.select_all(action='DESELECT')
for item in coll.objects:item.select_set(True)
bpy.context.view_layer.objects.active=root
bpy.ops.export_scene.fbx(filepath=str(out/'ShoreFoam.fbx'),use_selection=True,object_types={'MESH','EMPTY'},axis_forward='-Z',axis_up='Y',apply_scale_options='FBX_SCALE_UNITS',bake_anim=True,bake_anim_use_all_actions=False,bake_anim_use_nla_strips=False,bake_anim_simplify_factor=0,path_mode='RELATIVE')
manifest=json.loads((BASE/'asset-manifest.json').read_text(encoding='utf-8-sig'));manifest['assets'].append({'name':'ShoreFoam','group':'Water','fbx':'Water/ShoreFoam.fbx','source':'Water/Sources~/ShoreFoam.blend','triangles':len(faces)*2,'vertices':len(verts),'dimensionsBlenderXYZ':[10.2,10.2,0],'animatedFBX':True,'sourceAnimated':True,'frames':[1,121],'fps':30,'unityAnimation':'TidalWash loop'})
(BASE/'asset-manifest.json').write_text(json.dumps(manifest,indent=2))
bpy.data.collections['OW_Cargo_Textiles']['export_group']='Cargo_Textiles'
bpy.data.libraries.write(str(out/'Sources~'/'OceanSurface.blend'),{bpy.data.collections['OW_OceanSurface']},path_remap='RELATIVE',fake_user=True,compress=True)
preview=bpy.data.scenes['OW_Harbor_Preview'];bpy.context.window.scene=preview
obj=bpy.data.objects.new('Display_ShoreFoam',None);obj.instance_type='COLLECTION';obj.instance_collection=coll;preview.collection.objects.link(obj);obj.location=(0,3,-.08);obj.scale=(2.4,1.9,1)
bpy.data.objects['Display_Cargo_Textiles'].location.x=.8
preview.frame_set(1);bpy.ops.wm.save_as_mainfile(filepath=str(BASE/'_Production'/'Sources~'/'HarborWorkshop.blend'))
bpy.ops.render.render(write_still=True)
# Same camera and scene for a fair Tier 1 / Tier 2 comparison.
for obj in preview.objects:
 if obj.instance_collection and obj.instance_collection.name.endswith('_T2'):
  obj.instance_collection=bpy.data.collections[obj.instance_collection.name[:-1]+'1']
preview.render.filepath=str(BASE/'_Production'/'Previews'/'Harbor_Tier1.png');bpy.ops.render.render(write_still=True)
print('Shore foam exported; matched tier previews rendered')

