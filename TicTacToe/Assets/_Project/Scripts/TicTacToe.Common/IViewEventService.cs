using System;

namespace TicTacToe.Common
{
    public interface IViewEventService
    {
        void AddListener<T>(EventHandler<T> callback)
            where T : IControllerEvent;

        void Invoke<T>(object sender, T eventData)
            where T : IViewEvent;

        void RemoveAllListeners<T>()
            where T : IViewEvent;

        void RemoveListener<T>(EventHandler<T> callback)
            where T : IControllerEvent;
    }
}
