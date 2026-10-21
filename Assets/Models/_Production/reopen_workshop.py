import bpy
bpy.context.window.scene=bpy.data.scenes['OW_Harbor_Preview']
bpy.context.scene.frame_set(1)
if hasattr(bpy.ops,'blendermcp'):
 bpy.ops.blendermcp.start_server()
for area in bpy.context.screen.areas:
 if area.type=='VIEW_3D':
  area.spaces.active.region_3d.view_perspective='CAMERA'
  area.spaces.active.shading.type='MATERIAL'
