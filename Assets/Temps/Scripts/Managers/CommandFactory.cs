using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public static class CommandFactory
    {
        public static KeyCode[] s_moveKey = { KeyCode.A, KeyCode.W, KeyCode.D, KeyCode.S };

        public static ICommand CreateCommand(string commandName)
        {
            switch(commandName)
            {
                case "Move":return new MoveCommand();
                case "Attack":return new AttackCommand();
                case "Aim":return new RotateTowardMouseCommand();

                default: return new EmptyCommand();
            }
        }


        public static ICommand CreateMoveCommand(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.W: 
                case KeyCode.A: 
                case KeyCode.D: 
                case KeyCode.S: 
                    return new MoveCommand();



                default: return new EmptyCommand();
            }
        }

        public static ICommand CreateMouseCommand(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.Mouse0: return new AttackCommand();

                default: return new EmptyCommand();
            }
        }

        public static ICommand CreateFaceCommand(Vector3 mousePosition)
        {
            return new EmptyCommand();
        }
    }
}

