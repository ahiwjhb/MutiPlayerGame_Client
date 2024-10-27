# nullable enable
using Core.MVVM.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiPlayerGame.UI.Chat
{

    public class ChatViewModel: ViewModel
    {
        public ObservableValue<string> ChatterName { get; } = new(string.Empty);

        public ObservableValue<string> ChatText { get; } = new(string.Empty);
    }
}
