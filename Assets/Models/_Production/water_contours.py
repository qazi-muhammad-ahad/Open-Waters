import bpy,math,numpy as np
from pathlib import Path
BASE=Path(r'E:\Github-Fork\Open-Waters-repo\Assets\Models')
n=512;y,x=np.mgrid[0:n,0:n].astype(float)/n
bend=.68*np.sin(2*math.pi*(x-2*y))+.24*np.sin(2*math.pi*(2*x+y))
a=2*math.pi*(5*x+3*y+bend);b=2*math.pi*(-3*x+7*y+.3*np.sin(2*math.pi*(x+y)))
r=.5+.5*np.sin(a);g=.5+.5*np.sin(b)
breaks=.5+.5*np.sin(2*math.pi*(2*x-3*y))
foam=np.clip((np.sin(a)-.88)/.12,0,1)*np.clip((breaks-.15)/.65,0,1)
rgba=np.stack([r,g,foam,np.ones_like(r)],axis=-1).astype(np.float32)
field=bpy.data.images['OceanPattern'];field.pixels.foreach_set(rgba.ravel());field.filepath_raw=str(BASE/'Water'/'OceanPattern.png');field.save()
nt=bpy.data.materials['OW_Water'].node_tree
mapping=next(n for n in nt.nodes if n.type=='MAPPING');mapping.inputs['Scale'].default_value=(2,2,1)
mixes=[n for n in nt.nodes if n.type=='MIX_RGB'];mixes[-1].inputs[2].default_value=(.14,.53,.57,1)
preview=bpy.data.scenes['OW_Harbor_Preview'];bpy.context.window.scene=preview;preview.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(BASE/'_Production'/'Sources~'/'HarborWorkshop.blend'))
preview.render.filepath=str(BASE/'_Production'/'Previews'/'Harbor_Tier2.png');bpy.ops.render.render(write_still=True)
# Three phase water-only samples, exact same camera; source animation evidence.
for f in [1,31,61]:
 preview.frame_set(f);preview.render.resolution_percentage=50;preview.render.filepath=str(BASE/'_Production'/'Previews'/('Harbor_Motion_%03d.png'%f));bpy.ops.render.render(write_still=True)
preview.render.resolution_percentage=100;preview.frame_set(1)
for obj in preview.objects:
 if obj.instance_collection and obj.instance_collection.name.endswith('_T2'):obj.instance_collection=bpy.data.collections[obj.instance_collection.name[:-1]+'1']
preview.render.filepath=str(BASE/'_Production'/'Previews'/'Harbor_Tier1.png');bpy.ops.render.render(write_still=True)
# Export latest water source material (shader-driven FBX unchanged).
bpy.data.libraries.write(str(BASE/'Water'/'Sources~'/'OceanSurface.blend'),{bpy.data.collections['OW_OceanSurface']},path_remap='RELATIVE',fake_user=True,compress=True)
print('Refined curved wave contours and rendered motion phases')
