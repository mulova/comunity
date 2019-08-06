//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;
using mulova.commons;

namespace comunity
{
/**
 * Enum index를 기반으로 하는 배열을 저장할 경우 사용하는 base class.
 * EnumDataInspector와 함께 사용된다.
 */
    [System.Serializable]
    public class EGameObject : EnumData
    {
        public GameObject obj;
        
        public EGameObject() : base () {
        }
        
        public EGameObject(Enum e) : base (e) {
        }
    }
}

