# nullable enable
using Core.MVVM.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiPlayerGame.UI.SceneLoading
{
    public class SceneLoadingViewModel : ViewModel, IProgress<float>
    {
        public ObservableValue<float> LoadingProcess { get; } = new();

        public void Report(float value) {
            LoadingProcess.Value = value;
        }
    }
}
