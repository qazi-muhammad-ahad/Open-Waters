import bpy, math, numpy as np
from pathlib import Path
BASE=Path(r'E:\Github-Fork\Open-Waters-repo\Assets\Models')
scene=bpy.data.scenes['OW_AssetWorkshop']
# Normalize UV layer names after joining manually constructed and primitive meshes.
for o in scene.objects:
 if o.type!='MESH' or o.name.startswith('Ocean'):continue
 layers=o.data.uv_layers
 if not layers:continue
 dst=layers[0]
 for other in list(layers)[1:]:
  for i in range(len(dst.data)):
   if dst.data[i].uv.length<.001 and other.data[i].uv.length>.001:dst.data[i].uv=other.data[i].uv
  layers.remove(other)
 dst.name='PaletteUV';dst.active_render=True
# Texture pixels use encoded color values; material swatches are scene linear.
colors=[m for m in bpy.data.materials if m.name.startswith('OW_') and m.name not in ['OW_Water','OW_HarborPalette']]
def enc(v): return 12.92*v if v<=.0031308 else 1.055*v**(1/2.4)-.055
im=bpy.data.images['HarborPalette'];px=[]
for y in range(128):
 for x in range(256):
  c=colors[min((y//32)*8+x//32,len(colors)-1)].diffuse_color;px.extend([enc(c[0]),enc(c[1]),enc(c[2]),1])
im.pixels.foreach_set(px);im.save()
# Foam consists of short, curved crest segments, never broad continuous white bands.
n=512;y,x=np.mgrid[0:n,0:n].astype(float)/n
bend=.22*np.sin(2*math.pi*(2*x-y))+.09*np.sin(2*math.pi*(x+3*y))
a=2*math.pi*(11*x+7*y+bend);b=2*math.pi*(-7*x+13*y+.12*np.sin(2*math.pi*(x+y)))
r=.5+.5*np.sin(a);g=.5+.5*np.sin(b)
breaks=.5+.5*np.sin(2*math.pi*(5*x-8*y)+.7*np.sin(2*math.pi*(2*x+y)))
foam=np.clip((np.sin(a)-.93)/.07,0,1)*np.power(breaks,4)
rgba=np.stack([r,g,foam,np.ones_like(r)],axis=-1).astype(np.float32)
field=bpy.data.images['OceanPattern'];field.pixels.foreach_set(rgba.ravel());field.save()
m=bpy.data.materials['OW_Water'];nt=m.node_tree;nt.nodes.clear()
out=nt.nodes.new('ShaderNodeOutputMaterial');p=nt.nodes.new('ShaderNodeBsdfPrincipled');p.inputs['Roughness'].default_value=.28;nt.links.new(p.outputs[0],out.inputs['Surface'])
tex=nt.nodes.new('ShaderNodeTexImage');tex.image=field
uv=nt.nodes.new('ShaderNodeTexCoord');mapping=nt.nodes.new('ShaderNodeMapping');nt.links.new(uv.outputs['UV'],mapping.inputs['Vector']);nt.links.new(mapping.outputs['Vector'],tex.inputs['Vector'])
for f,v in [(1,(0,0,0)),(121,(1,0,0))]:mapping.inputs['Location'].default_value=v;mapping.inputs['Location'].keyframe_insert(data_path='default_value',frame=f)
split=nt.nodes.new('ShaderNodeSeparateColor');nt.links.new(tex.outputs['Color'],split.inputs[0])
base=nt.nodes.new('ShaderNodeMixRGB');base.inputs[1].default_value=(.012,.25,.34,1);base.inputs[2].default_value=(.018,.31,.38,1);nt.links.new(split.outputs[0],base.inputs[0])
foam=nt.nodes.new('ShaderNodeMixRGB');foam.inputs[2].default_value=(.3,.7,.7,1);nt.links.new(split.outputs[2],foam.inputs[0]);nt.links.new(base.outputs[0],foam.inputs[1]);nt.links.new(foam.outputs[0],p.inputs['Base Color'])
bump=nt.nodes.new('ShaderNodeBump');bump.inputs['Strength'].default_value=.12;bump.inputs['Distance'].default_value=.035;nt.links.new(split.outputs[1],bump.inputs['Height']);nt.links.new(bump.outputs[0],p.inputs['Normal'])
preview=bpy.data.scenes['OW_Harbor_Preview'];bpy.context.window.scene=preview
bpy.data.lights['OW_Sun'].energy=8500
ld=bpy.data.lights.new('OW_Fill','AREA');o=bpy.data.objects.new('OW_Fill',ld);preview.collection.objects.link(o);o.location=(8,0,15);ld.energy=3000;ld.size=15
from mathutils import Vector
o.rotation_euler=(-o.location).to_track_quat('-Z','Y').to_euler()
preview.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(BASE/'_Production'/'Sources~'/'HarborWorkshop.blend'))
bpy.ops.render.render(write_still=True)
print('Corrected palette UVs, color encoding, water crest scale; rendered')
