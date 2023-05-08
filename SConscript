from building import *
import rtconfig
import os

src = []
inc = []
group = []

cwd = GetCurrentDir() # get current dir path
rtt_cwd = cwd + '/LVGL.Simulator/lvgl/env_support/rt-thread/'
lv_conf_cwd = cwd + '/LVGL.Simulator/'

port_src = Glob('LVGL.Simulator/lvgl/env_support/rt-thread/*.c')
port_inc = [cwd]
port_inc += [rtt_cwd]
port_inc += [lv_conf_cwd]
group = group + DefineGroup('LVGL-port', port_src, depend = [''], CPPPATH = port_inc)

# check if .h or .hpp files exsit
def check_h_hpp_exsit(path):
    file_dirs = os.listdir(path)
    for file_dir in file_dirs:
        if os.path.splitext(file_dir)[1] in ['.h', '.hpp']:
            return True
    return False

#lvgl_cwd = cwd + '/../../'
lvgl_cwd = cwd + '/LVGL.Simulator/lvgl/'
lvgl_src_cwd = lvgl_cwd + 'src/'
inc = inc + [lvgl_src_cwd] + [lvgl_cwd]
for root, dirs, files in os.walk(lvgl_src_cwd):
    for dir in dirs:
        current_path = os.path.join(root, dir)
        src = src + Glob(os.path.join(current_path,'*.c')) # add all .c files
        if check_h_hpp_exsit(current_path): # add .h and .hpp path
            inc = inc + [current_path]


if GetDepend('PKG_LVGL_USING_EXAMPLES'):
    lvgl_src_cwd = lvgl_cwd + 'examples/'
    inc = inc + [lvgl_src_cwd]
    for root, dirs, files in os.walk(lvgl_src_cwd):
        for dir in dirs:
            current_path = os.path.join(root, dir)
            src = src + Glob(os.path.join(current_path,'*.c'))
            if check_h_hpp_exsit(current_path):
                inc = inc + [current_path]

if GetDepend('PKG_LVGL_USING_DEMOS'):
    lvgl_src_cwd = lvgl_cwd + 'demos/'
    inc = inc + [lvgl_src_cwd]
    for root, dirs, files in os.walk(lvgl_src_cwd):
        for dir in dirs:
            current_path = os.path.join(root, dir)
            src = src + Glob(os.path.join(current_path,'*.c'))
            if check_h_hpp_exsit(current_path):
                inc = inc + [current_path]

LOCAL_CFLAGS = ''
if rtconfig.PLATFORM == 'gcc' or rtconfig.PLATFORM == 'armclang': # GCC or Keil AC6
    LOCAL_CFLAGS += ' -std=c99'
elif rtconfig.PLATFORM == 'armcc': # Keil AC5
    LOCAL_CFLAGS += ' --c99 --gnu'
    
group = group + DefineGroup('LVGL', src, depend = [''], CPPPATH = inc)
#group = group + DefineGroup('LVGL', src, depend = [''], CPPPATH = inc, LOCAL_CFLAGS = LOCAL_CFLAGS)

list = os.listdir(cwd)
for d in list:
    path = os.path.join(cwd, d)
    if os.path.isfile(os.path.join(path, 'SConscript')):
        group = group + SConscript(os.path.join(d, 'SConscript'))

Return('group')