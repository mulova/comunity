#name!/usr/bin/python

import sys, getopt
import xml.etree.ElementTree as ET

from defs import *

namespace="http://schemas.microsoft.com/developer/msbuild/2003"

def set_assembly_name( xmlroot, assembly_name ):
    print 'set assembly name: ' + assembly_name
    for child in xmlroot.iter('{{{}}}AssemblyName'.format(namespace)):
        print 'change project name: ' + child.text + ' -> ' + assembly_name
        child.text=assembly_name


def find_node( root, node_name ):
    return root.find('{{{}}}{}'.format(namespace, node_name))

def set_custom_command( root_node, shell_script ):
    print 'set custom command: ' + shell_script
    for child in root_node.iter('{{{}}}PropertyGroup'.format(namespace)):
        if find_node(child, 'CustomCommands') is None:
            child1 = ET.SubElement(child, 'CustomCommands')
            child2 = ET.SubElement(child1, 'CustomCommands')
            child3 = ET.SubElement(child2, 'Command')
        else:
            child1 = find_node(child, 'CustomCommands')
            child2 = find_node(child1, 'CustomCommands')
            child3 = find_node(child2, 'Command')
        child3.set('type', 'AfterBuild')
        child3.set('command', shell_script)
        child3.set('workingdir', '${SolutionDir}')
        child3.set('externalConsole', 'True')
            

def remove_custom_command( root_node ):
    for child in root_node.iter('{{{}}}PropertyGroup'.format(namespace)):
        cmd = find_node(child, 'CustomCommands')
        if cmd is not None:
            child.remove(cmd)

def set_defines( root_node, add ):
    for child in root_node.iter('{{{}}}PropertyGroup'.format(namespace)):
        node = find_node(child, 'DefineConstants')
        if node is not None and len(add) > 0:
            node.text = node.text + 'UNITY_{};UNITY_{}_API;'.format(add.upper(), add.upper())


def remove_platform_defines( root_node, remove ):
    removes=['UNITY_'+remove, 'UNITY_'+remove+'_API']
    remove_defines(root_node, removes)

def remove_defines( root_node, removes):
    for child in root_node.iter('{{{}}}PropertyGroup'.format(namespace)):
        node = find_node(child, 'DefineConstants')
        if node is not None:
            #remove defines
            tokens=node.text.split(';')
            dst = ''
            for tok in tokens:
                if tok not in removes and len(tok) > 0: 
                    dst = dst + tok + ';'
            node.text=dst

def run_mod( postbuild_cmd, define=None):
    ET.register_namespace('', namespace)

    if 'editor_project_file' in globals():
        pfile=editor_project_file+'.csproj'
        tree = ET.parse(pfile)
        root = tree.getroot()
        set_assembly_name(root, editor_project)
        if postbuild_cmd is not None:
            if define:
                set_custom_command(root, postbuild_cmd + ' ' + define)
            else:
                set_custom_command(root, postbuild_cmd)
        else:
            remove_custom_command(root)
        if define is not None:
            remove_platform_defines(root, 'ANDROID')
            remove_platform_defines(root, 'IOS')
            remove_platform_defines(root, 'WEBGL')
            set_defines(root, define)
        tree.write(pfile)

    if 'runtime_project_file' in globals():
        pfile=runtime_project_file+'.csproj'
        tree = ET.parse(pfile)
        root = tree.getroot()
        set_assembly_name(root, runtime_project)
        if postbuild_cmd is None:
            remove_custom_command(root)
        if define is not None:
            remove_defines(root, ['UNITY_EDITOR', 'UNITY_EDITOR_OSX', 'UNITY_EDITOR_WIN', 'UNITY_EDITOR_64'])
            remove_platform_defines(root, 'ANDROID')
            remove_platform_defines(root, 'IOS')
            remove_platform_defines(root, 'WEBGL')
            set_defines(root, define)
        tree.write(pfile)

