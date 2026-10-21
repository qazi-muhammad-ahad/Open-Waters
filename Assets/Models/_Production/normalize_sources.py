import bpy,json,math
from pathlib import Path
from mathutils import Vector
BASE=Path(r'E:\Github-Fork\Open-Waters-repo\Assets\Models')
entries=json.loads((BASE/'asset-manifest.json').read_text(encoding='utf-8-sig'))['assets']
for entry in entries:
 path=BASE/entry['source']
 bpy.ops.wm.read_factory_settings(use_empty=True)
 with bpy.data.libraries.load(str(path),link=False) as (data,loaded):loaded.collections=data.collections
 scene=bpy.context.scene;scene.name=entry['name']+'_Source'
 for coll in loaded.collections:scene.collection.children.link(coll)
 scene.render.engine='BLENDER_EEVEE';scene.render.fps=30;scene.frame_start=1;scene.frame_end=121;scene.frame_set(1)
 # Texture paths become portable relative paths on save.
 coords=[o.matrix_world@Vector(v) for o in scene.objects if o.type=='MESH' for v in o.bound_box]
 lo=Vector([min(v[i] for v in coords) for i in range(3)]);hi=Vector([max(v[i] for v in coords) for i in range(3)]);center=(lo+hi)/2
 camdata=bpy.data.cameras.new('SourceCamera');cam=bpy.data.objects.new('SourceCamera',camdata);scene.collection.objects.link(cam);scene.camera=cam
 camdata.type='ORTHO';camdata.ortho_scale=max(hi-lo)*1.6;cam.location=center+Vector((8,-12,14));cam.rotation_euler=(center-cam.location).to_track_quat('-Z','Y').to_euler()
 for name,pos,power,size in [('Key',(-4,-6,10),1700,7),('Fill',(5,2,8),1000,8)]:
  ld=bpy.data.lights.new(name,'AREA');o=bpy.data.objects.new(name,ld);scene.collection.objects.link(o);o.location=center+Vector(pos);ld.energy=power;ld.size=size;o.rotation_euler=(center-o.location).to_track_quat('-Z','Y').to_euler()
 scene.world=bpy.data.worlds.new('Studio');scene.world.use_nodes=True
 bg=next(n for n in scene.world.node_tree.nodes if n.type=='BACKGROUND');bg.inputs[0].default_value=(.3,.4,.5,1);bg.inputs[1].default_value=.6
 scene.render.resolution_x=384;scene.render.resolution_y=384;scene.render.resolution_percentage=100;scene.render.film_transparent=True;scene.render.image_settings.file_format='PNG';scene.view_settings.view_transform='AgX'
 if entry['group']=='Icons':
  cam.location=center+Vector((0,-.01,10));cam.rotation_euler=(center-cam.location).to_track_quat('-Z','Y').to_euler();camdata.ortho_scale=1.95
  scene.render.filepath=str(BASE/'Icons'/(entry['name']+'.png'));bpy.ops.render.render(write_still=True)
 elif entry['group'].startswith('Cargo_') or entry['group']=='MapMarkers':
  scene.render.filepath=str(BASE/entry['group']/('CargoIcon_'+entry['name']+'.png' if entry['group'].startswith('Cargo_') else entry['name']+'.png'));bpy.ops.render.render(write_still=True)
 bpy.context.preferences.filepaths.save_version=0
 temporary=path.with_name(path.stem+'_normalized.blend')
 bpy.ops.wm.save_as_mainfile(filepath=str(temporary))
 temporary.replace(path)
print('Normalized',len(entries),'directly-openable sources and rendered 16 transparent icons')

