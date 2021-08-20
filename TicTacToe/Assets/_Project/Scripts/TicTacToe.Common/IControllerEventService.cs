using System;

namespace TicTacToe.Common
{
    public interface IControllerEventService
    {
        void AddListener<T>(EventHandler<T> callback);

        void Invoke<T>(object sender, T eventData)
            where T : IControllerEvent;

        void RemoveAllListeners<T>()
            where T : IControllerEvent;

        void RemoveListener<T>(EventHandler<T> callback);
    }
}
