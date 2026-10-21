"""Original Open Waters art kit. Run through Blender MCP. Metres, Z up, ship -Y forward.
Only creates OW_ collections in a dedicated scene; never modifies pre-existing scenes.
"""
import bpy, math, json, os, random
from mathutils import Vector
from pathlib import Path
BASE = Path(r'E:\Github-Fork\Open-Waters-repo\Assets\Models')
random.seed(2701)
TAU=math.tau
assets={}; current=None; root=None
palette={
 'Plaster':('#f4dfb4',.85), 'Stone':('#739ca3',.85), 'StoneLight':('#aec8c0',.9),
 'Roof':('#147d83',.55), 'RoofLight':('#26b4ae',.5), 'RoofDark':('#12535e',.7),
 'Timber':('#794e37',.8), 'Wood':('#bc8852',.8), 'Deck':('#dcb47a',.8),
 'Brass':('#efb44c',.4), 'Iron':('#304555',.6), 'Glass':('#80dbdf',.25),
 'Red':('#d65e47',.6), 'Cream':('#fff1ce',.8), 'Grass':('#6da36b',1),
 'GrassLight':('#98bd71',1), 'Rock':('#6b8794',1), 'Sand':('#ddc695',1),
 'Grain':('#eac575',.9), 'Textile':('#b67ab2',.9), 'Water':('#168fba',.25),
 'Foam':('#bdeee8',.5), 'Dark':('#182f41',.85)}
mats={}
def srgb(x):
 return x/12.92 if x<.04045 else ((x+.055)/1.055)**2.4
for name,(hx,rough) in palette.items():
 m=bpy.data.materials.new('OW_'+name); m.use_nodes=True
 rgb=tuple(srgb(int(hx[i:i+2],16)/255) for i in (1,3,5))
 m.diffuse_color=(*rgb,1)
 p=next(n for n in m.node_tree.nodes if n.type=='BSDF_PRINCIPLED')
 p.inputs['Base Color'].default_value=(*rgb,1); p.inputs['Roughness'].default_value=rough
 if name=='Brass': p.inputs['Metallic'].default_value=.25
 mats[name]=m
scene=bpy.data.scenes.new('OW_AssetWorkshop')
bpy.context.window.scene=scene
scene.unit_settings.system='METRIC'; scene.render.fps=30; scene.frame_start=1; scene.frame_end=121

def asset(name,group,description):
 global current,root
 current=bpy.data.collections.new('OW_'+name); scene.collection.children.link(current)
 root=bpy.data.objects.new(name,None); current.objects.link(root)
 assets[name]={'collection':current,'root':root,'group':group,'description':description,'clips':[]}
 return root

def finish(o,name,mat,parent=True):
 o.name=name
 for c in list(o.users_collection): c.objects.unlink(o)
 current.objects.link(o)
 if parent: o.parent=root
 if mat: o.data.materials.append(mats[mat])
 return o

def cube(name,loc,scale,mat,bevel=.06):
 bpy.ops.mesh.primitive_cube_add(size=1,location=loc); o=bpy.context.object; o.scale=scale
 bpy.ops.object.transform_apply(location=False,rotation=False,scale=True)
 finish(o,name,mat)
 if bevel:
  mod=o.modifiers.new('Soft crafted edges','BEVEL'); mod.width=bevel; mod.segments=2
  bpy.ops.object.modifier_apply(modifier=mod.name)
  mod=o.modifiers.new('Weighted face normals','WEIGHTED_NORMAL')
  bpy.ops.object.modifier_apply(modifier=mod.name)
 return o

def cyl(name,loc,r,depth,mat,vertices=12,r2=None):
 bpy.ops.mesh.primitive_cone_add(vertices=vertices,radius1=r,radius2=r if r2 is None else r2,depth=depth,location=loc)
 return finish(bpy.context.object,name,mat)

def ico(name,loc,scale,mat,sub=1):
 bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=sub,radius=1,location=loc)
 o=bpy.context.object; o.scale=scale; bpy.ops.object.transform_apply(location=False,rotation=False,scale=True)
 return finish(o,name,mat)

def mesh(name,verts,faces,mat):
 m=bpy.data.meshes.new(name); m.from_pydata(verts,[],faces); m.update()
 o=bpy.data.objects.new(name,m); current.objects.link(o); o.parent=root
 if mat: m.materials.append(mats[mat])
 return o

def beam(name,a,b,width,mat):
 a,b=Vector(a),Vector(b); o=cube(name,(a+b)/2,(width,width,(b-a).length),mat,.025)
 o.rotation_euler=(b-a).to_track_quat('Z','Y').to_euler(); return o

def ring(name,loc,major,minor,mat):
 bpy.ops.mesh.primitive_torus_add(major_segments=20,minor_segments=6,location=loc,major_radius=major,minor_radius=minor)
 return finish(bpy.context.object,name,mat)

def roof(x,y,z,w,d,h):
 # Broad overhanging gable with layered standing seams.
 verts=[(x-w/2,y-d/2,z),(x+w/2,y-d/2,z),(x,y-d/2,z+h),(x-w/2,y+d/2,z),(x+w/2,y+d/2,z),(x,y+d/2,z+h)]
 mesh('Folded teal roof',verts,[(0,1,2),(3,5,4),(0,3,4,1),(0,2,5,3),(1,4,5,2)],'Roof')
 beam('Brass ridge',(x,y-d/2-.07,z+h+.03),(x,y+d/2+.07,z+h+.03),.15,'Brass')
 for yy in [y-d/2+i*d/6 for i in range(7)]:
  for sign in [-1,1]: beam('Raised roof seam',(x,yy,z+h+.025),(x+sign*w/2,yy,z+.025),.065,'RoofLight')
 for yy in [y-d/2,y+d/2]:
  for sign in [-1,1]: beam('Roof fascia',(x,yy,z+h),(x+sign*w/2,yy,z),.13,'RoofDark')

def window(x,y,z,w=.55,h=.65):
 cube('Deep window frame',(x,y,z),(w+.16,.12,h+.16),'Timber',.025)
 cube('Aqua window',(x,y-.075,z),(w,.04,h),'Glass',.015)
 cube('Window mullion',(x,y-.11,z),(.05,.045,h),'Cream',.008)
 cube('Window sill',(x,y-.12,z-h/2-.08),(w+.23,.22,.1),'Deck',.02)

def foundation(w,d):
 cube('Beveled stone footing',(0,0,.18),(w,d,.36),'Stone',.16)
 cube('Foundation lip',(0,0,.38),(w+.12,d+.12,.12),'StoneLight',.06)

def bollard(x,y,z=.5):
 cyl('Mooring post',(x,y,z+.18),.13,.36,'Iron')
 cube('Mooring horns',(x,y,z+.35),(.5,.19,.13),'Brass',.035)
 cyl('Bollard base',(x,y,z),.26,.1,'Iron')

def crate(x,y,z,s=1,material='Wood'):
 cube('Cargo crate',(x,y,z+s*.5),(s,s,s),material,.045)
 for xx in [x-s*.44,x+s*.44]:
  cube('Crate vertical binding',(xx,y-s*.51,z+s/2),(.09,.045,s*.96),'Timber',.01)
 for zz in [z+.07,z+s-.07]: cube('Crate horizontal binding',(x,y-s*.52,zz),(s,.05,.1),'Timber',.01)
 beam('Diagonal crate batten',(x-s*.4,y-s*.54,z+.12),(x+s*.4,y-s*.54,z+s-.12),.07,'Deck')

def barrel(x,y,z,mat='Red'):
 cyl('Barrel body',(x,y,z+.44),.32,.88,mat)
 for zz in [z+.12,z+.72]: cyl('Barrel hoop',(x,y,zz),.34,.07,'Iron')
 cyl('Barrel lid',(x,y,z+.9),.29,.04,'Brass')

def animate(o,path,keys,clip='Idle'):
 for f,v in keys:
  setattr(o,path,v); o.keyframe_insert(data_path=path,frame=f)
 if o.animation_data and o.animation_data.action: o.animation_data.action.name=o.name+'_'+clip
 if clip not in assets[root.name]['clips']: assets[root.name]['clips'].append(clip)

def flag(x,y,z):
 cyl('Flagstaff',(x,y,z/2),.055,z,'Brass',8)
 o=mesh('Harbor pennant',[(x,y,z),(x+.72,y,z-.08),(x+.55,y,z-.37),(x,y,z-.42)],[(0,1,2,3)],'Red')
 # Mesh-local pivot is world origin; gentle cloth via shape key, kept in source and FBX.
 o.shape_key_add(name='Basis'); k=o.shape_key_add(name='Breeze')
 k.data[1].co.y+=.18; k.data[2].co.y-=.12
 for f,v in [(1,0),(31,1),(61,0),(91,1),(121,0)]: k.value=v; k.keyframe_insert(data_path='value',frame=f)
 if 'Breeze' not in assets[root.name]['clips']: assets[root.name]['clips'].append('Breeze')

# Five structures, exactly two tiers.
for tier in [1,2]:
 t=tier==2
 asset('Dock_T'+str(tier),'Dock','One berth; '+('reinforced quay with animated crane' if t else 'timber pier with mooring posts'))
 if t:
  foundation(6.8,5.2)
  for i in range(12): cube('Quay paving',(-3+i*.55,0,.48),(.51,4.9,.15),'StoneLight',.035)
 else:
  for x in [-2.65,2.65]:
   for y in [-2,0,2]: cyl('Timber pile',(x,y,-.05),.22,1.4,'Timber',10)
  for i in range(16): cube('Pier deck plank',(0,-2.3+i*.3,.5),(5.9,.275,.2),'Deck' if i%3 else 'Wood',.035)
  for x in [-2.6,2.6]: beam('Under deck bearer',(x,-2.5,.2),(x,2.5,.2),.25,'Timber')
 for x in [-2.55,2.55]:
  for y in [-1.9,1.9]: bollard(x,y,.65)
 for x in [-1.8,0,1.8]:
  o=ring('Quay rubber fender',(x,-2.65,.17),.32,.1,'Iron'); o.rotation_euler.x=math.pi/2
 if t:
  cyl('Crane pedestal',(1.8,.9,1.0),.48,1.0,'Iron')
  beam('Crane tower',(1.8,.9,1),(1.8,.9,3.5),.28,'Brass')
  beam('Crane jib',(1.8,.9,3.4),(-.8,.9,3.4),.22,'Brass')
  beam('Crane diagonal',(1.8,.9,2.1),(-.6,.9,3.4),.12,'Brass')
  beam('Hoist cable',(-.6,.9,3.3),(-.6,.9,1.4),.035,'Iron')
  hook=ring('Hoist ring',(-.6,.9,1.3),.17,.05,'Iron'); hook.rotation_euler.x=math.pi/2
  animate(hook,'location',[(1,(-.6,.9,1.3)),(61,(-.6,.9,2.1)),(121,(-.6,.9,1.3))],'Hoist')
  crate(-1.7,1.2,.65,.8)

 asset('Warehouse_T'+str(tier),'Warehouse','Open-sided grain store' if not t else 'Expanded twin-bay warehouse with cupola and cargo stacks')
 w,d=(3.8,3) if not t else (5.6,3.8)
 foundation(w+.7,d+.6)
 cube('Store plaster shell',(0,.25,1.55),(w,d-.4,2.2),'Plaster',.1)
 for x in [-w*.34,w*.34]:
  cube('Loading door recess',(x,-d/2+.42,1.35),(1.1,.12,1.75),'RoofDark',.03)
  for j in range(4): cube('Door slat',(x-.42+j*.28,-d/2+.33,1.35),(.24,.1,1.7),'Wood',.025)
  cube('Door lintel',(x,-d/2+.25,2.28),(1.35,.18,.19),'Timber',.025)
 for x in [-w/2,w/2]: cube('Corner timber',(x,-d/2+.4,1.55),(.2,.2,2.4),'Timber',.025)
 roof(0,.2,2.72,w+.65,d+.65,1.0)
 cube('Warehouse plaque',(0,-d/2-.16,2.55),(1,.12,.28),'Brass',.04)
 if t:
  cube('Vent cupola',(0,.4,3.75),(1.15,1.05,.62),'Cream',.04); roof(0,.4,4.05,1.5,1.4,.42)
  crate(w/2+.45,.8,.47,.68); crate(w/2+.45,.8,1.15,.62)
 else: crate(-1.7,-1.5,.47,.65)

 asset('Fuel_T'+str(tier),'Fuel','Small pump and drums' if not t else 'Twin-tank depot with gauge, manifold and canopy')
 foundation(3.8 if t else 3,3)
 for x in ([-.85,.85] if t else [0]):
  cyl('Fuel tank',(x,.45,1.3),.65,1.7,'Cream')
  cyl('Tank teal band',(x,.45,1.4),.665,.42,'Roof')
  cyl('Tank top',(x,.45,2.19),.66,.15,'Brass',12,.35)
  cyl('Filler vent',(x,.45,2.4),.12,.32,'Iron',10)
 cube('Fuel pump',(-.75,-.95,1.0),(.55,.5,1.15),'Red',.09)
 cube('Pump display',(-.75,-1.22,1.23),(.36,.04,.3),'Glass',.015)
 hose=ring('Coiled pump hose',(-.23,-1.0,.9),.3,.045,'Iron'); hose.rotation_euler.x=math.pi/2
 if t:
  for x in [-1.6,1.6]: beam('Canopy post',(x,-1.1,.45),(x,-1.1,2.7),.14,'Iron')
  roof(0,-.1,2.7,3.9,3,.5)
 else: barrel(.95,-.8,.46)

 asset('Office_T'+str(tier),'Office','Harbormaster permit hut' if not t else 'GSA office with annex, clock tower and permit board')
 foundation(4.8 if t else 3.5,3.3)
 cube('Office walls',(0,.15,1.6),(3,2.7,2.3),'Plaster',.1)
 cube('Door frame',(0,-1.25,1.25),(.8,.15,1.8),'Timber',.03)
 cube('Painted door',(0,-1.35,1.25),(.62,.07,1.6),'Roof',.025)
 cyl('Door knob',(.2,-1.42,1.2),.055,.07,'Brass',8).rotation_euler.x=math.pi/2
 for x in [-.95,.95]: window(x,-1.25,1.8,.5,.65)
 roof(0,.15,2.8,3.6,3.3,.95)
 cube('Office doorstep',(0,-1.65,.47),(1.25,.7,.25),'StoneLight',.08)
 if t:
  cube('Records annex',(1.9,.2,1.2),(1.3,2.5,1.5),'Plaster',.08); roof(1.9,.2,2,1.65,2.8,.5)
  cube('Clock lantern',(0,.3,3.85),(.9,.9,1),'Cream',.04); roof(0,.3,4.35,1.3,1.3,.5)
  c=cyl('Clock face',(0,-.18,3.95),.32,.06,'RoofDark',16); c.rotation_euler.x=math.pi/2
  beam('Clock long hand',(0,-.23,3.95),(0,-.23,4.18),.045,'Brass')
  beam('Clock short hand',(0,-.23,3.95),(.16,-.23,3.95),.045,'Brass')
 flag(-1.55,.9,4.2 if t else 3.6)
 cube('Permit board',(-1.7,-1.5,1.25),(.55,.13,.85),'RoofDark',.03)
 for zz in [1.05,1.25,1.45]: cube('Permit paper',(-1.7,-1.58,zz),(.37,.03,.11),'Cream',.005)

 asset('Repair_T'+str(tier),'Repair','Open maintenance platform with tools' if not t else 'Covered repair workshop and gantry')
 foundation(4.5,3.5)
 cube('Workbench',(0,.7,1.1),(2.8,.8,.16),'Deck',.04)
 for x in [-1.1,1.1]: cube('Bench legs',(x,.7,.75),(.18,.65,.7),'Iron',.02)
 gear=ring('Service gear',(0,.7,1.28),.35,.12,'Iron')
 for i in range(8):
  a=i*TAU/8; o=cube('Gear tooth',(.42*math.cos(a),.7+.42*math.sin(a),1.28),(.2,.2,.16),'Brass',.02)
 barrel(-1.5,-.65,.45,'Roof')
 cube('Tool chest',(1.2,-.55,.83),(1,.65,.75),'Red',.06)
 for z in [.65,.85,1.05]: cube('Drawer handle',(1.2,-.9,z),(.4,.05,.04),'Brass',.01)
 if t:
  for x in [-1.9,1.9]:
   for y in [-1.25,1.25]: beam('Shelter column',(x,y,.45),(x,y,2.8),.18,'Iron')
  roof(0,0,2.85,4.7,3.8,.85)
  beam('Service gantry',(-1.9,0,2.6),(1.9,0,2.6),.16,'Brass')
  cart=cube('Gantry trolley',(0,0,2.5),(.5,.35,.22),'Red',.03)
  animate(cart,'location',[(1,(-1,0,2.5)),(61,(1,0,2.5)),(121,(-1,0,2.5))],'Service')

# Single freighter: broad readable hull and deck, sockets retained as empties.
asset('CoastalFreighter','Freighter','Single starter ship; teal hull, ivory bridge, brass crane, cargo and wake sockets')
outline=[(-.75,-3.25),(.75,-3.25),(1.22,-2.45),(1.22,2.5),(.9,2.9),(-.9,2.9),(-1.22,2.5),(-1.22,-2.45)]
v=[(x*.76,y*.92,-.52) for x,y in outline]+[(x,y,.35) for x,y in outline]
mesh('Faceted steel hull',v,[tuple(range(7,-1,-1)),tuple(range(8,16))]+[(i,(i+1)%8,(i+1)%8+8,i+8) for i in range(8)],'RoofDark')
mesh('Timber main deck',[(x*.95,y*.97,.39) for x,y in outline],[tuple(range(8))],'Deck')
for i in range(8):
 a=outline[i];b=outline[(i+1)%8];beam('Cream gunwale',(*a,.42),(*b,.42),.14,'Cream')
for y in [-2.2,-1.7,-1.2,-.7,-.2,.3,.8,1.3,1.8,2.3]: beam('Deck seam',(-.98,y,.4),(.98,y,.4),.025,'Wood')
cube('Cargo hatch',(0,-.8,.53),(1.8,2.5,.22),'Iron',.08)
cube('Hatch cover',(0,-.8,.68),(1.62,2.3,.12),'Roof',.04)
cube('Bridge cabin',(0,1.65,1.02),(1.7,1.7,1.12),'Cream',.15)
for x in [-.5,0,.5]: window(x,.77,1.27,.34,.35)
cube('Bridge roof',(0,1.65,1.65),(2,2,.16),'Roof',.08)
cyl('Funnel',(.45,2.05,2.03),.23,.64,'Red')
cyl('Funnel cap',(.45,2.05,2.37),.27,.12,'Iron')
beam('Navigation mast',(-.4,1.8,1.7),(-.4,1.8,2.8),.065,'Brass')
beam('Radar crossbar',(-.8,1.8,2.6),(0,1.8,2.6),.06,'Cream')
for x in [-.96,.96]:
 for y in [1,1.7,2.4]: beam('Safety stanchion',(x,y,.43),(x,y,.85),.045,'Cream')
 beam('Safety rail',(x,1,.85),(x,2.4,.85),.045,'Cream')
 o=ring('Lifebuoy',(x,1.9,.95),.24,.07,'Red');o.rotation_euler.y=math.pi/2
beam('Deck crane',(.85,-.05,.5),(.85,-.05,1.9),.12,'Brass')
beam('Deck crane arm',(.85,-.05,1.9),(-.2,-.7,1.9),.1,'Brass')
flag(-.4,1.8,2.9)
for name,loc in [('CargoSocket',(0,-.8,.76)),('WakeSocket',(0,3,.03)),('DamageSocket',(.45,2.05,2.5)),('BowSocket',(0,-3.2,.1))]:
 o=bpy.data.objects.new(name,None);current.objects.link(o);o.parent=root;o.location=loc
animate(root,'location',[(1,(0,0,0)),(31,(0,0,.045)),(61,(0,0,0)),(91,(0,0,-.045)),(121,(0,0,0))],'Idle_Bob')
animate(root,'rotation_euler',[(1,(0,.015,0)),(31,(.012,0,0)),(61,(0,-.015,0)),(91,(-.012,0,0)),(121,(0,.015,0))],'Idle_Bob')

for kind in ['Grain','Textiles','MachineParts']:
 asset('Cargo_'+kind,'Cargo_'+kind,'Load-sized cargo cluster for freighter socket; also renderable as icon')
 for y in [-.5,0,.5]: cube('Pallet deck',(0,y,.08),(1.5,.23,.12),'Deck',.02)
 for x in [-.55,.55]: cube('Pallet bearer',(x,0,.035),(.13,1.35,.07),'Timber',.01)
 if kind=='Grain':
  for x,y,z in [(-.4,-.3,.45),(.35,-.3,.45),(-.35,.3,.45),(.35,.3,.45),(0,0,.98)]:
   ico('Grain sack',(x,y,z),(.32,.28,.4),'Grain',2)
   cyl('Sack tie',(x,y,z+.37),.1,.12,'Timber',8)
 elif kind=='Textiles':
  for z in [.36,.78]:
   for x in [-.36,.36]:
    cube('Folded textile bale',(x,0,z),(.65,1.05,.37),'Textile' if x<0 else 'Cream',.1)
    for y in [-.32,.32]: cube('Bale binding',(x,y,z),(.68,.065,.39),'Brass',.015)
 else:
  crate(0,0,.15,1.05,'Roof')
  o=ring('Exposed machine wheel',(0,0,1.3),.32,.1,'Iron')
  for i in range(8):
   a=i*TAU/8;cube('Machined cog tooth',(.39*math.cos(a),.39*math.sin(a),1.3),(.17,.17,.18),'Brass',.02)

# Modular dressing.
asset('Bollard','Props','Mooring prop');bollard(0,0,0)
asset('Barrel','Props','Painted harbor barrel');barrel(0,0,0)
asset('Crate','Props','Bound timber crate');crate(0,0,0)
asset('Container','Props','Ribbed cargo container')
cube('Container shell',(0,0,.7),(2.6,1.3,1.4),'Red',.08)
for x in [-1.15+i*.23 for i in range(11)]: cube('Container rib',(x,-.68,.7),(.07,.08,1.25),'Brass',.015)
for x in [-1.3,1.3]: cube('Container corner',(x,0,.7),(.1,1.4,1.5),'Iron',.02)
asset('HarborLamp','Props','Warm navigational lantern')
cyl('Lamp base',(0,0,.1),.28,.2,'Stone')
beam('Lamp post',(0,0,.1),(0,0,2.1),.11,'Iron')
cube('Lantern glass',(0,0,2.2),(.35,.35,.45),'Grain',.03)
for x in [-.2,.2]:
 for y in [-.2,.2]: beam('Lantern frame',(x,y,1.95),(x,y,2.45),.045,'Iron')
cyl('Lantern cap',(0,0,2.5),.37,.2,'Roof',4,0)
asset('Fence','Props','Two-metre repeatable waterfront rail')
for x in [-1,1]: cube('Fence post',(x,0,.55),(.15,.15,1.1),'Timber',.03)
for z in [.4,.85]: beam('Fence rail',(-1.05,0,z),(1.05,0,z),.1,'Deck')
asset('RockCluster','Coast','Faceted coastal rocks')
for x,y,s in [(-.5,0,1),(.5,.25,.7),(.2,-.5,.45)]: ico('Basalt boulder',(x,y,.3*s),(.8*s,.65*s,.65*s),'Rock',1)
asset('CoastalPine','Vegetation','Tiered wind-shaped coastal pine')
cyl('Pine trunk',(0,0,.7),.15,1.4,'Timber',7)
for z,r in [(1,1),(1.55,.8),(2.1,.55)]: cyl('Pine crown',(0,0,z+.3),r,1.2,'Grass' if z!=1.55 else 'GrassLight',7,0)
asset('CoastTile','Coast','Eight-metre modular island tile with layered stone, sand and grass')
for z,r,d,mat in [(-.65,5.0,1.3,'Rock'),(-.05,4.85,.4,'Sand'),(.18,4.55,.3,'Grass')]:
 cyl('Island terrace',(0,0,z),r,d,mat,12,r*.99)
asset('QuayTile','Coast','Four-metre repeatable quay foundation')
foundation(4,4)
for x in [-1.5,-.5,.5,1.5]:
 for y in [-1.5,-.5,.5,1.5]: cube('Paving slab',(x,y,.49),(.94,.94,.12),'StoneLight',.025)

# Destinations are compact map markers, not extra playable environments.
for name,kind in [('BeaconBay','lighthouse'),('Copperport','warehouse'),('MeridianQuay','tower')]:
 asset('Map_'+name,'MapMarkers','Miniature destination marker for route map')
 cyl('Map island',(0,0,.12),1,.24,'Sand',10)
 if kind=='lighthouse':
  cyl('Lighthouse',(0,0,1.0),.35,1.55,'Cream',10,.23)
  cyl('Lighthouse band',(0,0,1.05),.31,.22,'Red',10,.29)
  cyl('Lantern',(0,0,1.9),.28,.35,'Glass',8);cyl('Beacon roof',(0,0,2.15),.4,.22,'Roof',8,0)
 elif kind=='warehouse':
  cube('Map store',(0,0,.65),(1.25,1,.85),'Plaster',.05);roof(0,0,1.1,1.55,1.3,.5)
 else:
  cube('Quay tower',(0,0,1),(.75,.75,1.7),'StoneLight',.08);roof(0,0,1.9,1.1,1.1,.65)

# Reusable mesh FX with authored four-second clips.
asset('FX_Selection','Effects','Selection ring, pulse loop')
o=ring('Selection halo',(0,0,.06),1,.035,'Brass')
animate(o,'scale',[(1,(1,1,1)),(61,(1.06,1.06,1)),(121,(1,1,1))],'Pulse')
asset('FX_Wake','Effects','Layered swept foam chevrons, loop')
for i in range(7):
 y=i*.5; w=.3+i*.12
 o=mesh('Wake ribbon', [(-w,y,0),(0,y+.22,0),(w,y,0),(w+.05,y+.1,0),(0,y+.34,0),(-w-.05,y+.1,0)],[(0,1,4,5),(1,2,3,4)],'Foam')
 animate(o,'location',[(1,(0,0,0)),(61,(0,.18,.015)),(121,(0,0,0))],'Wake')
asset('FX_Arrival','Effects','Expanding mooring ripple')
for i in range(3):
 o=ring('Arrival ring',(0,0,.03+i*.008),.6+i*.35,.02,'Foam')
 animate(o,'scale',[(1,(.6,.6,1)),(91,(1.4,1.4,1)),(121,(.6,.6,1))],'Arrival')
asset('FX_Upgrade','Effects','Brass construction spark burst, four-second one-shot')
for i in range(12):
 a=TAU*i/12;o=ico('Upgrade spark',(math.cos(a),math.sin(a),.5),(.08,.08,.18),'Brass')
 animate(o,'location',[(1,(math.cos(a),math.sin(a),.1)),(70,(2*math.cos(a),2*math.sin(a),2.5)),(121,(2.5*math.cos(a),2.5*math.sin(a),.1))],'Upgrade')
 animate(o,'scale',[(1,(.01,.01,.01)),(30,(1,1,1)),(90,(1,1,1)),(121,(.01,.01,.01))],'Upgrade')
asset('FX_Loading','Effects','Crate lift and settle loop')
crate(0,0,0,.6)
animate(root,'location',[(1,(0,0,0)),(40,(0,0,1.5)),(81,(0,.8,1.5)),(121,(0,0,0))],'Loading')
asset('FX_DamageSmoke','Effects','Faceted smoke puffs for damaged-state socket')
for i in range(5):
 o=ico('Smoke puff',(.08*i,0,.2+i*.25),(.2+i*.02,)*3,'Stone',1)
 animate(o,'scale',[(1,(.8,.8,.8)),(61,(1.2,1.2,1.2)),(121,(.8,.8,.8))],'Smoke')

# Event / HUD emblems: sculptural, readable from above, with restrained attention pulse.
for kind in ['Storm','Pirates','EngineFault','Congestion','Inspection','Credits','Reputation','Suspicion','Permit','Route']:
 asset('Icon_'+kind,'Icons','Original raised emblem; mesh plus transparent icon render')
 cyl('Emblem backing',(0,0,.06),.8,.12,'RoofDark',12)
 if kind=='Storm':
  for x,y,s in [(-.3,0,.35),(0,.12,.42),(.35,0,.28)]: ico('Storm cloud',(x,y,.25),(s,s,.2),'StoneLight',2)
  mesh('Lightning bolt',[(-.05,-.05,.5),(.22,-.05,.5),(.02,-.35,.5),(.18,-.35,.5),(-.15,-.68,.5),(-.07,-.4,.5),(-.24,-.4,.5)],[(0,1,2,3,4,5,6)],'Brass')
 elif kind=='Pirates':
  ico('Skull',(0,.12,.3),(.38,.32,.2),'Cream',2)
  for x in [-.14,.14]: ico('Eye socket',(x,.1,.48),(.09,.1,.03),'Dark')
  beam('Crossbones',(-.4,-.48,.25),(.4,-.12,.25),.1,'Cream');beam('Crossbones',(-.4,-.12,.25),(.4,-.48,.25),.1,'Cream')
 elif kind=='EngineFault':
  ring('Engine cog',(0,0,.25),.36,.13,'Brass')
  for i in range(8):
   a=TAU*i/8;cube('Cog tooth',(.47*math.cos(a),.47*math.sin(a),.25),(.18,.18,.18),'Brass',.02)
 elif kind=='Congestion':
  for y in [-.4,0,.4]:
   cube('Queue dash',(-.1,y,.23),(.75,.16,.16),'Cream',.03);ico('Queue dot',(.45,y,.23),(.1,.1,.1),'Red')
 elif kind in ['Inspection','Permit']:
  cube('Permit document',(0,0,.22),(.75,1,.13),'Cream',.04)
  for y in [-.25,0,.25]: cube('Document line',(-.05,y,.3),(.42,.045,.025),'Roof',.005)
  if kind=='Inspection':
   ring('Inspection lens',(.22,-.15,.37),.24,.045,'Brass');beam('Lens handle',(.4,-.3,.37),(.61,-.56,.37),.09,'Brass')
  else: cyl('Permit seal',(.22,-.3,.34),.16,.08,'Red',10)
 elif kind=='Credits':
  for x,y,z in [(-.2,.15,.22),(.2,-.18,.3)]:
   cyl('Credit coin',(x,y,z),.34,.14,'Brass',16);ring('Coin rim',(x,y,z+.08),.27,.025,'Cream')
 elif kind=='Reputation':
  verts=[(math.cos(i*math.pi/5)*(.57 if i%2==0 else .25),math.sin(i*math.pi/5)*(.57 if i%2==0 else .25),.3) for i in range(10)]
  mesh('Reputation star',verts,[tuple(range(10))],'Brass')
 elif kind=='Suspicion':
  mesh('Warning triangle',[(-.57,-.45,.25),(.57,-.45,.25),(0,.55,.25)],[(0,1,2)],'Red')
  cube('Warning stroke',(0,.03,.31),(.1,.4,.05),'Cream',.02);cyl('Warning dot',(0,-.3,.32),.06,.05,'Cream',8)
 else:
  for x,y in [(-.4,-.3),(0,0),(.4,.3)]: cyl('Route node',(x,y,.25),.12,.12,'Brass',10)
  beam('Route segment',(-.4,-.3,.25),(.4,.3,.25),.045,'Cream')
 if kind in ['Storm','Pirates','EngineFault','Congestion','Inspection','Suspicion']:
  animate(root,'scale',[(1,(1,1,1)),(61,(1.07,1.07,1.07)),(121,(1,1,1))],'Attention')

# Ocean grid plus four phase targets: authored cyclic swells, no random displacement.
asset('OceanSurface','Water','32m water tile; periodic analytic swells and directional foam; matching URP shader')
N=64;size=32
verts=[(-size/2+size*x/N,-size/2+size*y/N,0) for y in range(N+1) for x in range(N+1)]
faces=[(y*(N+1)+x,y*(N+1)+x+1,(y+1)*(N+1)+x+1,(y+1)*(N+1)+x) for y in range(N) for x in range(N)]
o=mesh('Ocean grid',verts,faces,'Water')
uv=o.data.uv_layers.new(name='UVMap')
for poly in o.data.polygons:
 poly.use_smooth=True
 for li in poly.loop_indices:
  co=o.data.vertices[o.data.loops[li].vertex_index].co;uv.data[li].uv=((co.x+16)/32,(co.y+16)/32)
def wave(x,y,p): return .12*math.sin(TAU*(2*x+y)/32+p)+.055*math.sin(TAU*(-x+3*y)/32-p*2)+.025*math.sin(TAU*(5*x+2*y)/32+p*3)
o.shape_key_add(name='Basis')
for j in range(4):
 k=o.shape_key_add(name='SwellPhase'+str(j))
 for v in k.data: v.co.z=wave(v.co.x,v.co.y,j*math.pi/2)
 for f in range(1,122,15):
  phase=(f-1)/120*4;k.value=max(0,1-min(abs(phase-j),abs(phase-j-4),abs(phase-j+4)));k.keyframe_insert(data_path='value',frame=f)
assets[root.name]['clips']=['OceanLoop']
# Continuous procedural Blender material, with long ordered wave crests and micro-ripples.
m=mats['Water'];nt=m.node_tree;p=next(n for n in nt.nodes if n.type=='BSDF_PRINCIPLED')
tex=nt.nodes.new('ShaderNodeTexCoord'); mapping=nt.nodes.new('ShaderNodeVectorMath');mapping.operation='SCALE';mapping.inputs[3].default_value=1
nt.links.new(tex.outputs['Generated'],mapping.inputs[0])
w=nt.nodes.new('ShaderNodeTexWave');w.wave_type='BANDS';w.bands_direction='DIAGONAL';w.inputs['Scale'].default_value=7;w.inputs['Distortion'].default_value=3;w.inputs['Detail Scale'].default_value=.7
nt.links.new(mapping.outputs[0],w.inputs['Vector'])
r=nt.nodes.new('ShaderNodeValToRGB');r.color_ramp.elements.remove(r.color_ramp.elements[1])
for pos,color in [(0,'#126385'),(.4,'#128ead'),(.8,'#22adc4'),(.94,'#83d9d5'),(1,'#c7f1e8')]:
 e=r.color_ramp.elements[0] if pos==0 else r.color_ramp.elements.new(pos);e.position=pos;e.color=(*[srgb(int(color[i:i+2],16)/255) for i in (1,3,5)],1)
nt.links.new(w.outputs['Fac'],r.inputs[0]);nt.links.new(r.outputs[0],p.inputs['Base Color'])
bump=nt.nodes.new('ShaderNodeBump');bump.inputs['Strength'].default_value=.15;bump.inputs['Distance'].default_value=.09;nt.links.new(w.outputs['Fac'],bump.inputs['Height']);nt.links.new(bump.outputs[0],p.inputs['Normal'])
w.inputs['Phase Offset'].default_value=0;w.inputs['Phase Offset'].keyframe_insert(data_path='default_value',frame=1)
w.inputs['Phase Offset'].default_value=TAU;w.inputs['Phase Offset'].keyframe_insert(data_path='default_value',frame=121)
scene.frame_set(1)
print('Created',len(assets),'assets;',sum(len(v['collection'].objects) for v in assets.values()),'objects')
