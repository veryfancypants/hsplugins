"""

QUICK START GUIDE

* Edit the "tfpath" variable below to point at your install of AI. Create the Timeline\Single Files folder if it does not exist.
* Run the script.
* Fire up NeoV2.
* Add a map (say, 'housing island') and a female character.
* Select the character. Kinematics -> FK & IK -> turn on 'function', uncheck 'Neck', then Kinematics -> Neck -> Anim.
* Hit Ctrl-T.
* Mouse-click "single files". You should see 'gen_output_char1' in the new window. Click on it. Click on 'load'.
* In the timeline window, hit 'stop' then 'play'. Observe.
* Alt-tab out of the game, read through the code starting with 'def do_script()'. 
* Tweak the script (say, change 'speed' to 2.0 in the call to gen_walk). Rerun the script.
* Alt-tab into the game. 'gen_output_char1' should still be selected. Click on 'load'. Restart playback or click anywhere on the black bar.
* Seek to 9.0 s. Open AIPE. Open the advanced mode editor. Click on 'f_t_arm_R(work)'. In the right pane, click on 'Position'. 
* Use the controls to position the hand right above the lips. Note the x,y,z numbers and put them in the call to tff.post('bonePos hand_r', 9.0,...)
* Rerun the script, reload the generated file. Verify that the hand travels to the right place at 9.0 s.

"""


# LIBRARY CODE 

import math
import random
import xml.etree.ElementTree as ET
tfpath="C:\\Program Files (x86)\\Steam\\steamapps\\common\\AI-Shoujo\\BepInEx\\plugins\\Timeline\\Single Files\\"
header='<root animationCategory="3" animationGroup="0" animationNo="0">'
cc='<curveKeyframe time="0" value="0" inTangent="0" outTangent="1" /><curveKeyframe time="1" value="1" inTangent="1" outTangent="0" /></keyframe>'
cc2='<curveKeyframe time="0" value="0" inTangent="0" outTangent="1" /><curveKeyframe time="1" value="1" inTangent="0" outTangent="0" /></keyframe>'
cc3='<curveKeyframe time="0" value="0" inTangent="0" outTangent="0" /><curveKeyframe time="1" value="1" inTangent="0" outTangent="0" /></keyframe>'
footer='</root>'

def gen_header_bonepos(x, y, z):
	return '<interpolable enabled="true" owner="AIPE_fork" objectIndex="' + str(x) + '" id="' + z + '" parameter="' + y + '" bgColorR="1" bgColorG="1" bgColorB="1" alias="">'
#def gen_header_bonerot(x, y):
#	return '<interpolable enabled="true" owner="AIPE" objectIndex="' + str(x) + '" id="boneRot" parameter="' + y + '" bgColorR="1" bgColorG="1" bgColorB="1" alias="">'
def get_header_pos(x, y):
	return '<interpolable enabled="true" owner="Timeline" objectIndex="' + str(x) + '" id="' + y + '" guideObjectPath="" bgColorR="1" bgColorG="1" bgColorB="1" alias="">'
def get_header_other(x, y):
	return '<interpolable enabled="true" owner="Timeline" objectIndex="' + str(x) + '" id="' + y + '" bgColorR="1" bgColorG="1" bgColorB="1" alias="">'

ikbones=[
	'f_t_hips(work)',
	'f_t_shoulder_L(work)',
	'f_t_elbo_L(work)',
	'f_t_arm_L(work)',
	'f_t_thigh_L(work)',
	'f_t_knee_L(work)',
	'f_t_leg_L(work)',
	'f_t_shoulder_R(work)',
	'f_t_elbo_R(work)',
	'f_t_arm_R(work)',
	'f_t_thigh_R(work)',
	'f_t_knee_R(work)',
	'f_t_leg_R(work)'
]

ikrot=['f_t_leg_L(work)', 'f_t_leg_R(work)']


def gen_rotator(t, v, suffix=cc):
	return ('<keyframe time="%f" valueX="%f" valueY="%f" valueZ="%f" valueW="%f">' % (t,v[0],v[1],v[2],v[3])) + suffix

def gen_rotator_2(t, *args): #phi, psi=0, theta=0, suffix=cc):
	if type(args[-1]) is str:
		suffix=args[-1]
		args=args[:-1]
	else:
		suffix=cc
	if len(args)>4:
		print("Too many arguments to gen_rotator_2", args)
		exit(-1)
	if len(args)<1:
		print("Too few arguments to gen_rotator_2", args)
		exit(-1)
	if len(args)==4:
		return gen_rotator(t, args, suffix=suffix)
	phi=args[0]
	psi=args[1] if len(args)>1 else 0.0
	theta=args[2] if len(args)>2 else 0.0
	rad=3.1415926/180.
	phi=phi*rad/2.
	psi=psi*rad/2.
	theta=theta*rad/2.
	s1, c1=math.sin(phi), math.cos(phi)
	s2, c2=math.sin(psi), math.cos(psi)
	s3, c3=math.sin(theta), math.cos(theta)
	return gen_rotator(t, [
		s1*c2*c3-c1*s2*s3,
		c1*s2*c3+s1*c2*s3,
		c1*c2*s3-s1*s2*c3,
		c1*c2*c3+s1*s2*s3], suffix=suffix)

def gen_v1(t, v, suffix=cc):
	return ('<keyframe time="%f" value="%f">' % (t,v) ) + suffix
def gen_v1i(t, v, suffix=cc):
	return ('<keyframe time="%f" value="%d">' % (t,v) ) + suffix
def gen_v3(t, x,y,z, suffix=cc):
	return ('<keyframe time="%f" valueX="%f" valueY="%f" valueZ="%f">' % (t,x,y,z) ) + suffix

def rng():
	return random.random()-0.5

reference_dim=[
14.1, # shoulder height
10.9, # hip height
9.4,  # thigh height
5.4,  # knee height
0.84, # leg width
1.24, # shoulder width
]

# Reference walking animation ("Walking 1") for an average male (see reference_dim above). 
positions={
('f_t_hips(work)', 0) : [0.018, 10.607, 0.310],('f_t_hips(work)', 1) : [0.017, 10.588, 0.318],('f_t_hips(work)', 9) : [0.029, 10.252, 0.367],('f_t_hips(work)', 11) : [0.031, 10.184, 0.368],
('f_t_hips(work)', 17) : [0.001, 10.316, 0.330],('f_t_hips(work)', 18) : [-0.003, 10.358, 0.322],('f_t_hips(work)', 27) : [0.009, 10.631, 0.278],('f_t_hips(work)', 37) : [0.029, 10.366, 0.338],
('f_t_hips(work)', 42) : [0.035, 10.179, 0.342],('f_t_hips(work)', 46) : [0.053, 10.181, 0.318],('f_t_hips(work)', 52) : [0.065, 10.436, 0.285],
('f_t_shoulder_L(work)', 0) : [-1.329, 13.708, 0.250],('f_t_shoulder_L(work)', 1) : [-1.340, 13.686, 0.271],('f_t_shoulder_L(work)', 9) : [-1.371, 13.339, 0.298],('f_t_shoulder_L(work)', 11) : [-1.367, 13.272, 0.275],
('f_t_shoulder_L(work)', 17) : [-1.325, 13.419, 0.090],('f_t_shoulder_L(work)', 18) : [-1.319, 13.463, 0.069],('f_t_shoulder_L(work)', 27) : [-1.259, 13.759, 0.086],('f_t_shoulder_L(work)', 37) : [-1.197, 13.509, 0.115],
('f_t_shoulder_L(work)', 42) : [-1.176, 13.311, 0.055],('f_t_shoulder_L(work)', 46) : [-1.167, 13.301, -0.033],('f_t_shoulder_L(work)', 52) : [-1.197, 13.535, -0.032],
('f_t_elbo_L(work)', 0) : [-1.914, 11.241, 0.267],('f_t_elbo_L(work)', 1) : [-1.923, 11.221, 0.368],('f_t_elbo_L(work)', 9) : [-1.961, 10.900, 0.657],('f_t_elbo_L(work)', 11) : [-1.967, 10.831, 0.601],
('f_t_elbo_L(work)', 17) : [-1.927, 10.956, 0.074],('f_t_elbo_L(work)', 18) : [-1.914, 11.000, 0.010],('f_t_elbo_L(work)', 27) : [-1.822, 11.363, -0.521],('f_t_elbo_L(work)', 37) : [-1.835, 11.194, -0.697],
('f_t_elbo_L(work)', 42) : [-1.821, 10.989, -0.731],('f_t_elbo_L(work)', 46) : [-1.798, 10.936, -0.692],('f_t_elbo_L(work)', 52) : [-1.799, 11.113, -0.482],
('f_t_arm_L(work)', 0) : [-2.461, 9.418, 1.333],('f_t_arm_L(work)', 1) : [-2.474, 9.416, 1.462],('f_t_arm_L(work)', 9) : [-2.544, 9.139, 1.805],('f_t_arm_L(work)', 11) : [-2.560, 9.030, 1.680],
('f_t_arm_L(work)', 17) : [-2.487, 8.916, 0.606],('f_t_arm_L(work)', 18) : [-2.459, 8.937, 0.464],('f_t_arm_L(work)', 27) : [-2.318, 9.241, -0.614],('f_t_arm_L(work)', 37) : [-2.409, 9.103, -0.931],
('f_t_arm_L(work)', 42) : [-2.399, 8.889, -0.843],('f_t_arm_L(work)', 46) : [-2.352, 8.827, -0.638],('f_t_arm_L(work)', 52) : [-2.312, 9.026, -0.112],
('f_t_thigh_L(work)', 0) : [-0.820, 9.098, 0.027],('f_t_thigh_L(work)', 1) : [-0.820, 9.081, 0.024],('f_t_thigh_L(work)', 9) : [-0.817, 8.773, -0.025],('f_t_thigh_L(work)', 11) : [-0.817, 8.708, -0.037],
('f_t_thigh_L(work)', 17) : [-0.819, 8.818, -0.045],('f_t_thigh_L(work)', 18) : [-0.820, 8.854, -0.040],('f_t_thigh_L(work)', 27) : [-0.820, 9.111, 0.029],('f_t_thigh_L(work)', 37) : [-0.820, 8.860, 0.076],
('f_t_thigh_L(work)', 42) : [-0.818, 8.676, 0.074],('f_t_thigh_L(work)', 46) : [-0.818, 8.687, 0.054],('f_t_thigh_L(work)', 52) : [-0.818, 8.947, 0.035],
('f_t_knee_L(work)', 0) : [-0.483, 5.110, 0.383],('f_t_knee_L(work)', 1) : [-0.509, 5.084, 0.293],('f_t_knee_L(work)', 9) : [-0.585, 4.782, -0.429],('f_t_knee_L(work)', 11) : [-0.587, 4.751, -0.692],
('f_t_knee_L(work)', 17) : [-0.665, 4.987, -1.247],('f_t_knee_L(work)', 18) : [-0.700, 4.978, -1.092],('f_t_knee_L(work)', 27) : [-0.784, 5.554, 1.897],('f_t_knee_L(work)', 37) : [-0.570, 5.602, 2.415],
('f_t_knee_L(work)', 42) : [-0.544, 5.281, 2.205],('f_t_knee_L(work)', 46) : [-0.505, 5.191, 2.011],('f_t_knee_L(work)', 52) : [-0.479, 5.229, 1.521],
('f_t_leg_L(work)', 0) : [-0.076, 0.784, -0.695],('f_t_leg_L(work)', 1) : [-0.080, 0.783, -0.872],('f_t_leg_L(work)', 9) : [-0.111, 0.787, -2.393],('f_t_leg_L(work)', 11) : [-0.120, 0.828, -2.798],
('f_t_leg_L(work)', 17) : [-0.259, 1.527, -4.058],('f_t_leg_L(work)', 18) : [-0.309, 1.665, -4.077],('f_t_leg_L(work)', 27) : [-1.041, 2.305, -1.172],('f_t_leg_L(work)', 37) : [-0.599, 1.193, 3.188],
('f_t_leg_L(work)', 42) : [-0.283, 1.015, 3.539],('f_t_leg_L(work)', 46) : [-0.123, 0.777, 2.651],('f_t_leg_L(work)', 52) : [-0.050, 0.780, 1.267],
('f_t_shoulder_R(work)', 0) : [1.294, 13.839, 0.063],('f_t_shoulder_R(work)', 1) : [1.283, 13.826, 0.081],('f_t_shoulder_R(work)', 9) : [1.253, 13.498, 0.144],('f_t_shoulder_R(work)', 11) : [1.258, 13.422, 0.127],
('f_t_shoulder_R(work)', 17) : [1.304, 13.522, 0.015],('f_t_shoulder_R(work)', 18) : [1.311, 13.559, 0.014],('f_t_shoulder_R(work)', 27) : [1.372, 13.827, 0.153],('f_t_shoulder_R(work)', 37) : [1.432, 13.560, 0.254],
('f_t_shoulder_R(work)', 42) : [1.454, 13.366, 0.180],('f_t_shoulder_R(work)', 46) : [1.462, 13.357, 0.078],('f_t_shoulder_R(work)', 52) : [1.434, 13.630, -0.038],
('f_t_elbo_R(work)', 0) : [1.845, 11.430, -0.501],('f_t_elbo_R(work)', 1) : [1.839, 11.418, -0.481],('f_t_elbo_R(work)', 9) : [1.843, 11.105, -0.449],('f_t_elbo_R(work)', 11) : [1.853, 11.029, -0.465],
('f_t_elbo_R(work)', 17) : [1.870, 11.080, -0.363],('f_t_elbo_R(work)', 18) : [1.870, 11.109, -0.317],('f_t_elbo_R(work)', 27) : [1.917, 11.394, 0.616],('f_t_elbo_R(work)', 37) : [1.939, 11.152, 0.864],
('f_t_elbo_R(work)', 42) : [1.972, 10.944, 0.717],('f_t_elbo_R(work)', 46) : [2.001, 10.908, 0.450],('f_t_elbo_R(work)', 52) : [1.963, 11.152, -0.084],
('f_t_arm_R(work)', 0) : [2.395, 9.367, -0.054],('f_t_arm_R(work)', 1) : [2.383, 9.341, -0.095],('f_t_arm_R(work)', 9) : [2.409, 9.003, -0.311],('f_t_arm_R(work)', 11) : [2.427, 8.928, -0.344],
('f_t_arm_R(work)', 17) : [2.423, 8.994, -0.049],('f_t_arm_R(work)', 18) : [2.418, 9.029, 0.048],('f_t_arm_R(work)', 27) : [2.441, 9.529, 1.618],('f_t_arm_R(work)', 37) : [2.438, 9.361, 2.005],
('f_t_arm_R(work)', 42) : [2.494, 9.107, 1.772],('f_t_arm_R(work)', 46) : [2.553, 8.992, 1.336],('f_t_arm_R(work)', 52) : [2.531, 9.170, 0.628],
('f_t_thigh_R(work)', 0) : [0.820, 9.075, 0.048],('f_t_thigh_R(work)', 1) : [0.820, 9.057, 0.054],('f_t_thigh_R(work)', 9) : [0.818, 8.715, 0.091],('f_t_thigh_R(work)', 11) : [0.817, 8.646, 0.091],
('f_t_thigh_R(work)', 17) : [0.818, 8.793, 0.060],('f_t_thigh_R(work)', 18) : [0.818, 8.837, 0.055],('f_t_thigh_R(work)', 27) : [0.820, 9.101, 0.029],('f_t_thigh_R(work)', 37) : [0.819, 8.842, 0.001],
('f_t_thigh_R(work)', 42) : [0.819, 8.662, -0.035],('f_t_thigh_R(work)', 46) : [0.819, 8.654, -0.057],('f_t_thigh_R(work)', 52) : [0.820, 8.891, -0.036],
('f_t_knee_R(work)', 0) : [0.653, 5.937, 2.551],('f_t_knee_R(work)', 1) : [0.615, 5.956, 2.601],('f_t_knee_R(work)', 9) : [0.486, 5.387, 2.318],('f_t_knee_R(work)', 11) : [0.514, 5.153, 2.053],
('f_t_knee_R(work)', 17) : [0.499, 5.217, 1.863],('f_t_knee_R(work)', 18) : [0.510, 5.233, 1.805],('f_t_knee_R(work)', 27) : [0.531, 5.167, 0.793],('f_t_knee_R(work)', 37) : [0.562, 4.833, -0.035],
('f_t_knee_R(work)', 42) : [0.565, 4.712, -0.726],('f_t_knee_R(work)', 46) : [0.617, 4.801, -1.181],('f_t_knee_R(work)', 52) : [0.972, 4.876, -0.012],
('f_t_leg_R(work)', 0) : [1.029, 2.212, 0.095],('f_t_leg_R(work)', 1) : [1.004, 1.994, 0.555],('f_t_leg_R(work)', 9) : [0.316, 1.120, 3.659],('f_t_leg_R(work)', 11) : [0.200, 1.021, 3.746],
('f_t_leg_R(work)', 17) : [0.077, 0.781, 2.300],('f_t_leg_R(work)', 18) : [0.074, 0.787, 2.092],('f_t_leg_R(work)', 27) : [0.031, 0.787, 0.016],('f_t_leg_R(work)', 37) : [0.070, 0.783, -1.878],
('f_t_leg_R(work)', 42) : [0.114, 0.831, -2.912],('f_t_leg_R(work)', 46) : [0.212, 1.186, -3.789],('f_t_leg_R(work)', 52) : [0.552, 2.306, -3.653],
}

rotations={
('f_t_leg_L(work)', 0):  [0.014, -0.018, -0.009, -0.999], ('f_t_leg_L(work)', 1):  [0.015, -0.017, -0.009, -0.999], 
('f_t_leg_L(work)', 9):  [0.008, -0.013, -0.013, -0.999], ('f_t_leg_L(work)', 11): [0.008, 0.013, 0.015, 0.999], 
('f_t_leg_L(work)', 17): [0.366, 0.006, 0.060, 0.928], ('f_t_leg_L(work)', 18): [0.428, 0.003, 0.079, 0.900], 
('f_t_leg_L(work)', 27): [0.398, -0.074, -0.079, 0.910], ('f_t_leg_L(work)', 37): [0.180, -0.018, -0.023, -0.983], 
('f_t_leg_L(work)', 42): [-0.263, -0.008, 0.054, 0.963], ('f_t_leg_L(work)', 46): [0.094, -0.008, -0.037, -0.994], 
('f_t_leg_L(work)', 52): [0.015, -0.017, -0.023, -0.999], 
('f_t_leg_R(work)', 0):  [0.269, 0.036, 0.056, 0.960], ('f_t_leg_R(work)', 1):  [0.213, 0.023, 0.052, 0.975], 
('f_t_leg_R(work)', 9):  [-0.277, -0.010, -0.069, 0.958], ('f_t_leg_R(work)', 11): [-0.301, 0.018, -0.079, 0.949], 
('f_t_leg_R(work)', 17): [-0.063, 0.029, -0.090, 0.993], ('f_t_leg_R(work)', 18): [-0.047, 0.031, -0.092, 0.994], 
('f_t_leg_R(work)', 27): [-0.013, 0.030, -0.026, 0.999], ('f_t_leg_R(work)', 37): [-0.012, 0.033, -0.016, 0.999], 
('f_t_leg_R(work)', 42): [-0.011, -0.037, 0.020, -0.999], ('f_t_leg_R(work)', 46): [-0.189, -0.042, 0.029, -0.980], 
('f_t_leg_R(work)', 52): [-0.585, -0.073, 0.090, -0.802], 
}


bone_shortcuts={
	'hand_l': 'f_t_arm_L(work)',
	'hand_r': 'f_t_arm_R(work)',
	'foot_l': 'f_t_leg_L(work)',
	'foot_r': 'f_t_leg_R(work)',
	'neck':'BodyTop/p_cf_anim/cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_Neck',
	'head':'BodyTop/p_cf_anim/cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_Neck/cf_J_Head',
}

class timefile:
	def __init__(self, name, id):
		self.name=name
		self.id=id
		self.streams={}
		self.constructors={}
		self.timestamps={}
		self.last={}
	def reformat(self, stream):
		if stream.startswith('bonePos ') or stream.startswith('boneRot ') or stream.startswith('boneScale '):
			x=stream.split()
			if x[1] in bone_shortcuts:
				x[1]=bone_shortcuts[x[1]]
			stream=' '.join(x)
		return stream
	def post(self, stream, *args):
		stream=self.reformat(stream)
		if not stream in self.streams:
			if stream.startswith('bonePos') or stream.startswith('boneScale'):
				x=stream.split()
				self.streams[stream]=gen_header_bonepos(self.id, x[1], x[0])
				self.constructors[stream]=gen_v3
			elif stream.startswith('boneRot'):
				x=stream.split()[1]
				self.streams[stream]=gen_header_bonepos(self.id, x, 'boneRot')
				self.constructors[stream]=gen_rotator_2
			elif stream=='pos':
				self.streams[stream]=get_header_pos(self.id,'guideObjectPos')
				self.constructors[stream]=gen_v3
			elif stream=='rot':
				self.streams[stream]=get_header_pos(self.id,'guideObjectRot')
				self.constructors[stream]=gen_rotator_2
			elif stream=='hand_r':
				self.streams[stream]=get_header_other(self.id,'characterRightHand')
				self.constructors[stream]=gen_v1i
			elif stream=='hand_l':
				self.streams[stream]=get_header_other(self.id,'characterLeftHand')
				self.constructors[stream]=gen_v1i
			elif stream=='mouth':
				self.streams[stream]=get_header_other(self.id,'characterMouth')
				self.constructors[stream]=gen_v1i
			elif stream=='mouthOpen':
				self.streams[stream]=get_header_other(self.id,'characterMouthOpen')
				self.constructors[stream]=gen_v1
			else:
				print("Unknown stream type", stream)
			self.timestamps[stream]=-1.0
		if type(args[0]) is str:
			data=args[0]
		else:
			if args[0]<0.0:
				print("Error: negative time")
			elif args[0]<=self.timestamps[stream]:
				print("Error: timestamps in ", stream, " must be in strictly increasing order")
			self.timestamps[stream]=args[0]
			if args[1]=='last':
				data=self.constructors[stream](args[0], *self.last[stream])
			else:
				self.last[stream]=args[1:]
				data=self.constructors[stream](*args)
		self.streams[stream]+=data
	def get_last(self, stream):
		return self.last[self.reformat(stream)]
	def flush(self):
		out = header
		for x in self.streams:
			out+=self.streams[x]
			out+="</interpolable>\n"
		out+=footer
		of=open(tfpath+self.name, "w")
		of.write(out)
		of.close()


def merge_point(tf, name, tt, newtime):
	tree = ET.parse(tfpath+name)
	root = tree.getroot()
	s={}
	for x in root:
		id=x.attrib['id']
		if id!='bonePos' and id!='boneRot' and id!="guideObjectPos" and id!="guideObjectRot":
			continue
		for y in x:
			#print id, y.attrib['time']
			if id=='bonePos' or id=='guideObjectPos':
				p, t, vx, vy, vz = (x.attrib['parameter'] if id=='bonePos' else None), float(y.attrib['time']), float(y.attrib['valueX']), float(y.attrib['valueY']), float(y.attrib['valueZ'])
				t=int(t*100)
				if t==tt:
					tf.post(id+' '+p if id=='bonePos' else 'pos', newtime, vx, vy, vz)
			else:#if id=="boneRot":
				p, t, vx, vy, vz, vw = (x.attrib['parameter'] if id=='boneRot' else None), float(y.attrib['time']), float(y.attrib['valueX']), float(y.attrib['valueY']), float(y.attrib['valueZ']), float(y.attrib['valueW'])
				t=int(t*100)
				if t==tt:
					tf.post(id+' '+p if id=='boneRot' else 'rot', newtime, vx, vy, vz, vw)


def gen_walk(tf,scale=None, stop=None, steps=5, phi=0.0, speed=1.0, stride_mult=1.0, start_point=[0.0, 0.0, 0.0]):
	keys=[0,9,17,27,37,42,46,52]
	if scale==None:
		scale=[1.,1.,1.,1.,1.,1.]
	elif type(scale) is float:
		scale=[scale,1.,1.,1.,1.,1.]
	for x in range(1,6):
		scale[x]*=scale[0]
	for n in range(steps):
		for nn in range(8):
			d=(n+keys[nn]/60.) * stride_mult*scale[2]
			t=(n+keys[nn]/60.)/speed
			if stop!=None and t>=stop:
				return
			for k in ikbones:
				pos = positions[(k, keys[nn])][:]
				if ('shoulder' in k) or ('elbo' in k) or ('arm' in k):
					pos[0] *= scale[5]
					pos[1] *= scale[0]
					pos[2] *= scale[5]
				elif ('hip' in k):
					pos[1] *= scale[1]
					pos[2] *= scale[1]
				elif ('thigh' in k):
					pos[0] *= scale[4]
					pos[1] *= scale[2]
				elif 'knee' in k:
					pos[0] *= scale[4]
					pos[1] *= scale[3]
					pos[2] *= stride_mult*scale[2]
				else:
					pos[2] = pos[2]*stride_mult*scale[2]
					pos[0] = pos[0]*scale[4] 
				tf.post('bonePos '+k, t, pos[0], pos[1], pos[2])
			for k in ikrot:
				pos = rotations[(k, keys[nn])]
				tf.post('boneRot '+k, t, *pos)
			v=13.0
			tf.post('pos', t, start_point[0]+d*v*math.sin(phi*3.14159/180.), start_point[1], start_point[2]+d*v*math.cos(phi*3.14159/180.))

def rotate_z(v, phi):
	c=math.cos(phi*3.14159/180.)
	s=math.sin(phi*3.14159/180.)
	return [v[0]*c+v[2]*s, v[1], -v[0]*s+v[2]*c]

#simple spin-in-place
def gen_spin(tf, t, angle, duration, steps=0):
	start_phi = tf.get_last('rot')[1]
	start_pos = tf.get_last('pos')
	start_lf = tf.get_last('bonePos foot_l')
	start_rf = tf.get_last('bonePos foot_r')
	tf.post('bonePos foot_l', t, *start_lf)
	tf.post('bonePos foot_r', t, *start_rf)
	tf.post('boneRot foot_l', t, 0.0)
	tf.post('boneRot foot_r', t, 0.0)
	if steps==0:
		steps=int(abs(angle)/90.+0.99)
	lf_lead=(angle<0.0)
	for n in range(steps):
		step_angle=angle/steps
		for phase in range(4):
			progress=(n+0.25*(phase+1))/steps
			tt = t + progress*duration
			tf.post('rot', tt, 0.0, start_phi+progress*angle)
			angles=[-1,-2,-1,0] if lf_lead else [1,2,1,0]
			lfnew = rotate_z(start_lf, -step_angle*0.25*angles[phase])
			rfnew = rotate_z(start_rf, step_angle*0.25*angles[phase])
			tf.post('bonePos foot_l', tt, lfnew[0], lfnew[1]+(0.2 if phase==(0 if lf_lead else 2) else 0.0), lfnew[2])
			tf.post('bonePos foot_r', tt, rfnew[0], rfnew[1]+(0.2 if phase==(2 if lf_lead else 0) else 0.0), rfnew[2])
			tf.post('boneRot foot_l', tt, 0.0, -step_angle*0.25*angles[phase])
			tf.post('boneRot foot_r', tt, 0.0, step_angle*0.25*angles[phase])

def gen_fancy_spin(tf, t, angle, duration, steps=5):
	start_phi = tf.get_last('rot')[1]
	start_pos = tf.get_last('pos')
	start_lf = tf.get_last('bonePos foot_l')
	start_rf = tf.get_last('bonePos foot_r')
	tf.post('bonePos foot_l', t, *start_lf)
	tf.post('bonePos foot_r', t, *start_rf)
	tf.post('boneRot foot_l', t, 0.0)
	tf.post('boneRot foot_r', t, 0.0)
	t += duration/(steps+3)
	tf.post('boneRot foot_l', t, 15.0, 0.0)
	tf.post('boneRot foot_r', t, 15.0, 0.0)
	foot_len=1.3
	s, c = math.sin(15.0*3.14159/180.), math.cos(15.0*3.14159/180.)
	dv = [0.0, -foot_len*s, foot_len*c]
	#dv = rotate_z(dv, start_phi)
	ball_lf = [start_lf[0]+dv[0], start_lf[1]+dv[1], start_lf[2]+dv[2]]
	ball_rf = [start_rf[0]+dv[0], start_rf[1]+dv[1], start_rf[2]+dv[2]]

	dv=[0.0, foot_len*s, +foot_len*(1.-c)]
	dv = rotate_z(dv, -start_phi)
	start_pos=list(start_pos)
	start_pos[0]+=dv[0]
	start_pos[1]+=dv[1]
	start_pos[2]+=dv[2]
	tf.post('pos', t, *start_pos)
	t += duration/(steps+3)
	lf_lead=(angle<0.0)
	for n in range(steps):
		progress=float(n)/steps
		tf.post('rot', t, 0.0, start_phi+progress*angle)
		tf.post('pos', t, *start_pos)
		lf = rotate_z(ball_lf, -progress*angle)
		rf = rotate_z(ball_rf, -progress*angle)
		lf[1] += foot_len*s
		rf[1] += foot_len*s
		lf[2] -= foot_len*c
		rf[2] -= foot_len*c
		tf.post('bonePos foot_l', t, *lf)
		tf.post('bonePos foot_r', t, *rf)
		t += duration/(steps+3)

	lf = rotate_z(ball_lf, -angle)
	rf = rotate_z(ball_rf, -angle)

	if angle<0:
		# drop the left heel:
		lf = [lf[0], lf[1], lf[2]-foot_len]
		dv=[0.0, -foot_len*s, -foot_len*(1.-c)]
		dv[0]-=start_lf[0]-lf[0]
		dv[2]-=start_lf[2]-lf[2]
		dv = rotate_z(dv, (start_phi+angle))
	else:
		rf = [rf[0], rf[1], rf[2]-foot_len]
		dv=[0.0, -foot_len*s, -foot_len*(1.-c)]
		dv[0]-=start_rf[0]-rf[0]
		dv[2]-=start_rf[2]-rf[2]
		dv = rotate_z(dv, (start_phi+angle))
	start_pos[0]+=dv[0]
	start_pos[1]+=dv[1]
	start_pos[2]+=dv[2]

	tf.post('pos', t, *start_pos)
	tf.post('rot', t, 0.0, start_phi+angle)
	tf.post('bonePos foot_l', t, *start_lf)
	tf.post('bonePos foot_r', t, *start_rf)
	tf.post('boneRot foot_l', t, 0.0)
	tf.post('boneRot foot_r', t, 0.0)



# LIBRARY CODE ENDS
# USER CODE BEGINS

def do_script():
	# the id number (2nd argument) usually does not matter
	tff = timefile('gen_output_char1.xml', 1)

	# The script issues a series of commands. Each command specifies the target (e.g. left knee),  the time, and any parameters,
	# and generates a Timeline keyframe. When the script is executed, the character will go through these commands, smoothly
	# interpolating between positions for continuous commands (bone position, rotation), or switching at specified time for
	# discrete commands (e.g. hand shapes).

	# Basic posing is done with targets:
	# 'pos' and 'rot' - position and rotation of the character as a whole
	# 'bonePos f_t_hips(work)', etc. (see the 'ikbones' list above) - positions of several body parts (feet, knees, thighs, 
	# hips, shoulders, elbows, wrists), which are (a bit confusingly) referred to as "IK bones" in the lingo
	# For feet and wrists, we also have 'boneRot' in addition to 'bonePos'.
	# We can use shortcuts 'hand_r', 'hand_l', foot_r', 'foot_l' instead of 'f_t_arm_R(work)', etc.
	# Positions are specified as x/y/z coordinates: x is left-right, y is up-down, and z is backwards-forwards. For bone positions,
	# the point 0,0,0 is between the character's feet. Rotations are specified as pitch/yaw/tilt angles, you'll probably 
	# want to use trial and error to determine which is which.

	# Other supported targets are:
	# 'mouth', 'mouthOpen', 'hand_l', 'hand_r' = mouth preset (integer), mouth openness (float 0 to 1), left hand shape preset, right hand shape preset
	# 'boneRot neck', 'boneRot head' - rotating the neck joint (base of the neck) and the head joint (top of the neck)
	#
	# The character has a large number of 'FK bones' ('neck' and 'head' are just two of them), you can, in principle, control any of them using any of 
	# the prefixes 'bonePos', 'boneRot', or 'boneScale', you just need to know the bone's true name. Typically, you use only boneRot with parts of the skeleton, 
	# but you may use bonePos and boneScale on bones that represent soft tissues. 

	# Posts to each target go in strictly increasing order.  Posts to different targets, or e.g. setting rotation and position
	# of the same target, don't need to be synchronized. 
	
# PART 1

	# turn 25 degrees clockwise around the vertical axis
	tff.post('rot', 0.0, 0.0, 25.0)
	# Rotate neck to default.
	tff.post('boneRot neck', 0.0, 0.0)

	tff.post('hand_l', 0.0, 13)
	tff.post('hand_r', 0.0, 8)

	# Turn palms more or less parallel to the body
	tff.post('boneRot hand_l', 0.0, 0.0, 0.0, 50.0)
	tff.post('boneRot hand_r', 0.0, 0.0, 0.0, -50.0)

	# walk for 5.5 seconds or the default 5 steps (whichever happens first),
	# at speed 1.0 (default), making half-length strides, starting at (-9.0, 2.2, 14.0) (change the second number to adjust for floor height),
	# in the direction of 25 degrees. 
	# We need to know body dimensions, or at least the overall body scale, to generate the walking animation. Pass 0.95 for an average female 
	# and 1.0 for an average male. The character will walk crouched if the scale is too low, and on straight legs with locked knees if it's too high.
	gen_walk(tff, stop=5.5, speed=1.0, stride_mult=0.5, start_point=[-9.0, 2.2, 14.0], phi=25.0, scale=0.95)

	# at 5.5 s, put the right foot down and turn it flat
	tff.post('bonePos foot_r', 5.5, 1.03, 0.88, 0.45)
	tff.post('boneRot foot_r', 5.5, 0.0)
	# adjust the knee 
	tff.post('bonePos f_t_knee_R(work)', 5.5, 3.6, 3.6, 3.6)
	# neck still straight
	tff.post('boneRot neck', 5.5, 0.0)
	
	# by 5.9 s, turn neck left 15 deg, up 10 deg
	tff.post('boneRot neck', 5.9, -10.0, -15.0)

	# starting at 6.0 s, spin in place by 180 degrees counter-clockwise, taking 1.5 s to complete the turn
	gen_spin(tff, 6.0, -180.0, 1.5)

	#gen_fancy_spin(tff, 6.0, -60.0, 0.5)
	#gen_fancy_spin(tff, 6.5, -60.0, 0.5)
	#gen_fancy_spin(tff, 7.0, -60.0, 0.5)
	
	# by 6.5 s, turn body to heading -125, tilt neck up 20
	#tff.post('rot', 6.5, 0.0, -125.0)
	tff.post('boneRot neck', 6.5, -20.0)

	# by 6.8 s, tilt neck up 30 degrees
	tff.post('boneRot neck', 6.8, -30.0)

	# right hand stays at the last given position through 7.2 s 
	# (without this, it would immediately start moving toward the next position, which is issued further down in part 2)
	tff.post('boneRot hand_r', 7.2, 'last')
	tff.post('bonePos hand_r', 7.2, 'last')
	tff.post('bonePos f_t_elbo_R(work)', 7.2, 'last')

# PART 2
	
#tilt head even further back

	tff.post('boneRot neck', 8.0, -25.0)

	tff.post('boneRot head', 7.5, 0.0)
	# I don't think it actually tilts that far IRL
	tff.post('boneRot head', 8.0, -60.0)

# From 7.0 s to 7.9 s, open mouth wide. At 8.0, switch to mouth shape 13 aka 'ridiculously wide'.

	tff.post('mouth', 0.0, 0)
	tff.post('mouthOpen', 0.0, 0.0)
	tff.post('mouthOpen', 7.0, 0.0)
	tff.post('mouthOpen', 7.9, 1.0)
	tff.post('mouth', 8.0, 13)

# Bring the right hand in front of the face by 7.5 s, and above the face at 8.0s.
	tff.post('bonePos hand_r', 7.5, 2.0, 12.0, 3.0)
	tff.post('boneRot hand_r', 7.5, 40.0, -90.0, 0.0)

	tff.post('bonePos hand_r', 8.0, 0.70, 16.8, 0.2)
	tff.post('bonePos f_t_elbo_R(work)', 8.0, 10.0, 11.0, 0.0)
	tff.post('boneRot hand_r', 8.0, 80.0, 180.0, 0.0)

# Between 7.5 s and 8.0 s, move the head down slightly (you're not supposed to do that to FK bones, but 
	tff.post('bonePos BodyTop/p_cf_anim/cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_Neck/cf_J_Head/cf_J_Head_s', 7.5, 0.0, 0.0, 0.0)
	tff.post('bonePos BodyTop/p_cf_anim/cf_J_Root/cf_N_height/cf_J_Hips/cf_J_Spine01/cf_J_Spine02/cf_J_Spine03/cf_J_Neck/cf_J_Head/cf_J_Head_s', 8.0, 0.0, -0.2, 0.0)

# Move the right hand down until it is just above the mouth. 
	tff.post('bonePos hand_r', 9.0, 0.64, 15.1, -0.1)
	tff.post('boneRot hand_r', 9.0, 85.0, 180.0, 0.0)

	#gen_spin(tff, 10.0, 270.0, 2.0)

	#for n in range(10):
	#	gen_fancy_spin(tff, 10.0+n*0.5, 50.0, 0.5)

	#A way to bring in fully hand-tuned poses.
	#* Pose the character.
	#* In Timeline, create rows for all affected parameters.
	#* Add keyframes for each with the same timestamp.
	#* Save the timeline file.
	#* Specify the name of the saved file, the timestamp at which to pull keyframes (times 100, as integer, rounded down), and the timestamp where to put them.
	#* For example, the following line will copy all supported keyframes it finds between 1.500 s and 1.509 s, and insert them into the current file at time 9.2 s.
	#
	#merge_point(tff, "source.xml", 150, 9.2)	
	
	tff.flush()

do_script()

