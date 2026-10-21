import bpy, math
from mathutils import Vector
from pathlib import Path
BASE=Path(r'E:\Github-Fork\Open-Waters-repo\Assets\Models')
scene=bpy.data.scenes.new('OW_Harbor_Preview');bpy.context.window.scene=scene
scene.render.engine='BLENDER_EEVEE';scene.render.resolution_x=1400;scene.render.resolution_y=1100;scene.render.resolution_percentage=100
scene.render.image_settings.file_format='PNG';scene.render.fps=30;scene.frame_end=121
scene.world=bpy.data.worlds.new('OW_StudioWorld');scene.world.use_nodes=True
bg=next(n for n in scene.world.node_tree.nodes if n.type=='BACKGROUND');bg.inputs[0].default_value=(.17,.25,.32,1);bg.inputs[1].default_value=.65

def instance(name,loc,scale=(1,1,1),rot=0):
 o=bpy.data.objects.new('Display_'+name,None);o.instance_type='COLLECTION';o.instance_collection=bpy.data.collections['OW_'+name];scene.collection.objects.link(o);o.location=loc;o.scale=scale;o.rotation_euler.z=rot;return o
instance('OceanSurface',(0,0,-.12),(1.7,1.7,1))
instance('CoastTile',(0,3,-.05),(2.4,1.9,1))
instance('Dock_T2',(0,-5.3,.12))
instance('Warehouse_T2',(-3,2.4,.3))
instance('Office_T2',(3.5,4.6,.3))
instance('Fuel_T2',(-5.5,-1.3,.3))
instance('Repair_T2',(3.6,-.5,.3))
instance('CoastalFreighter',(0,-9,.1),rot=math.pi/2)
instance('Cargo_Textiles',(-.8,-9,.85),(.9,.9,.9),math.pi/2)
instance('FX_Wake',(-4.5,-9,-.02),rot=-math.pi/2)
for x,y,s in [(-8,4,1),(-7,6,1.3),(0,8,1.1),(5,7,.8),(8,3,1.1)]:instance('CoastalPine',(x,y,.3),(s,s,s))
for x,y in [(-8,-.4),(7,-1.8),(-4,-4),(5,6.7)]:instance('RockCluster',(x,y,.1))
for x,y in [(-2,-2.7),(2,-2.7),(-3.8,5.4),(5,2)]:instance('HarborLamp',(x,y,.3))
instance('Container',(-3.8,5.8,.3));instance('Cargo_Grain',(-2.4,-1,.35));instance('Cargo_MachineParts',(2,-2,.35))
for x in [-3,-1,1,3]:instance('Fence',(x,7.5,.3))
camdata=bpy.data.cameras.new('OW_Isometric');cam=bpy.data.objects.new('OW_Isometric',camdata);scene.collection.objects.link(cam)
target=Vector((0,0,0));cam.location=(22,-29,33);cam.rotation_euler=(target-cam.location).to_track_quat('-Z','Y').to_euler();camdata.type='ORTHO';camdata.ortho_scale=31;scene.camera=cam
ld=bpy.data.lights.new('OW_Sun','AREA');light=bpy.data.objects.new('OW_Sun',ld);scene.collection.objects.link(light);light.location=(-10,-12,22);ld.energy=4200;ld.shape='DISK';ld.size=12
light.rotation_euler=(Vector((0,0,0))-light.location).to_track_quat('-Z','Y').to_euler()
scene.view_settings.view_transform='AgX'
scene.render.film_transparent=False
preview=BASE/'_Production'/'Previews';preview.mkdir(exist_ok=True)
scene.render.filepath=str(preview/'Harbor_Tier2.png')
scene.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(BASE/'_Production'/'Sources~'/'HarborWorkshop.blend'))
bpy.ops.render.render(write_still=True)
print('Preview rendered')
