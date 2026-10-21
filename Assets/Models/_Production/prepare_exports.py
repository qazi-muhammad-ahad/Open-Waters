import bpy, math, json
from pathlib import Path
BASE=Path(r'E:\Github-Fork\Open-Waters-repo\Assets\Models')
scene=bpy.data.scenes['OW_AssetWorkshop'];bpy.context.window.scene=scene
scene.frame_set(1)
# A single swatch atlas prevents one draw call per decorative material.
colors=[m for m in bpy.data.materials if m.name.startswith('OW_') and m.name!='OW_Water']
img=bpy.data.images.new('HarborPalette',width=256,height=128,alpha=True)
pixels=[0.0]*(256*128*4)
for yy in range(128):
 for xx in range(256):
  idx=min((yy//32)*8+xx//32,len(colors)-1); c=colors[idx].diffuse_color
  # Image data are linear when assigned through pixels; output PNG handles encoding.
  off=(yy*256+xx)*4;pixels[off:off+4]=c
img.pixels.foreach_set(pixels)
shared=BASE/'Shared';shared.mkdir(exist_ok=True)
img.filepath_raw=str(shared/'HarborPalette.png');img.file_format='PNG';img.save()
mat=bpy.data.materials.new('OW_HarborPalette');mat.use_nodes=True
p=next(n for n in mat.node_tree.nodes if n.type=='BSDF_PRINCIPLED');p.inputs['Roughness'].default_value=.68
tex=mat.node_tree.nodes.new('ShaderNodeTexImage');tex.image=img;tex.interpolation='Closest';mat.node_tree.links.new(tex.outputs['Color'],p.inputs['Base Color'])
lookup={m.name:i for i,m in enumerate(colors)}
for coll in scene.collection.children:
 if coll.name=='OW_OceanSurface':continue
 for obj in coll.objects:
  if obj.type!='MESH':continue
  uv=obj.data.uv_layers.active or obj.data.uv_layers.new(name='PaletteUV')
  for face in obj.data.polygons:
   old=obj.data.materials[face.material_index];idx=lookup[old.name]
   u=((idx%8)+.5)/8;v=((idx//8)+.5)/4
   for li in face.loop_indices:uv.data[li].uv=(u,v)
   face.material_index=0
  obj.data.materials.clear();obj.data.materials.append(mat)
 # Join static geometry while retaining animated children and sockets.
 statics=[o for o in coll.objects if o.type=='MESH' and not o.animation_data and not o.data.shape_keys]
 if len(statics)>1:
  bpy.ops.object.select_all(action='DESELECT')
  for o in statics:o.select_set(True)
  bpy.context.view_layer.objects.active=statics[0];bpy.ops.object.join();statics[0].name=coll.name[3:]+'_Static'
# Deterministic seamless foam field: bent wave bands, nested ripples, interrupted crest ribbons.
import numpy as np
n=512;y,x=np.mgrid[0:n,0:n].astype(float)/n
bend=.18*np.sin(2*math.pi*(2*x-y))+.07*np.sin(2*math.pi*(x+3*y))
a=2*math.pi*(7*x+4*y+bend);b=2*math.pi*(-5*x+9*y+.12*np.sin(2*math.pi*(x+y)))
r=.5+.5*np.sin(a);g=.5+.5*np.sin(b)
crest=np.clip((np.sin(a)-.80)/.20,0,1)
breaks=.5+.5*np.sin(2*math.pi*(3*x-5*y)+.7*np.sin(2*math.pi*(2*x+y)))
foam=np.clip(crest*(.6+.4*breaks)+.2*np.power(g,12),0,1)
rgba=np.stack([r,g,foam,np.ones_like(r)],axis=-1).astype(np.float32)
field=bpy.data.images.new('OceanPattern',width=n,height=n,alpha=True);field.colorspace_settings.name='Non-Color';field.pixels.foreach_set(rgba.ravel());field.filepath_raw=str(BASE/'Water'/'OceanPattern.png');field.file_format='PNG';field.save()
# Persist metadata on collections so export can resume independently.
for coll in scene.collection.children:
 name=coll.name[3:]
 if name.endswith(('_T1','_T2')):group=name.rsplit('_T',1)[0]
 elif name.startswith('Cargo_'):group=name
 elif name.startswith('FX_'):group='Effects'
 elif name.startswith('Icon_'):group='Icons'
 elif name.startswith('Map_'):group='MapMarkers'
 elif name=='CoastalFreighter':group='Freighter'
 elif name=='OceanSurface':group='Water'
 elif name in ['RockCluster','CoastTile','QuayTile']:group='Coast'
 elif name=='CoastalPine':group='Vegetation'
 else:group='Props'
 coll['export_group']=group
 coll['animated']=any(o.animation_data or (o.type=='MESH' and o.data.shape_keys and o.data.shape_keys.animation_data) for o in coll.objects)
# Save a durable native master without touching pre-existing .blend files.
out=BASE/'_Production'/'Sources~'/'HarborWorkshop.blend'
bpy.ops.wm.save_as_mainfile(filepath=str(out))
print('Saved consolidated source',out.name,'mesh objects',sum(o.type=='MESH' for o in scene.objects))

