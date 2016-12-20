namespace UnityEngine.PlaymodeTests
{
    using System;
    using UnityEngine.PlaymodeTestsRunner;

    public class PlaymodeTest
    {
        public static void Fail()
        {
            PlaymodeTestsController.GetController().FailPlaymodeTest();
        }

        public static void Pass()
        {
            PlaymodeTestsController.GetController().PassPlaymodeTest();
        }
    }
}

