using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class BarbarianCharacter : BaseCharacter
    {
        public Animator anim;
        public Rigidbody rbody;

        private void Start()
        {
            anim = GetComponent<Animator>();
            rbody = GetComponent<Rigidbody>();
        }
    }

}
