using System;

namespace Zenject.Commands
{
    public interface ICommand
    {
    }

    public abstract class Command : ICommand
    {
        Action _handler;

        [PostInject]
        public void Construct(Action handler)
        {
            _handler = handler;
        }

        public void Execute()
        {
            _handler();
        }
    }

    public abstract class Command<T1> : ICommand
    {
        Action<T1> _handler;

        [PostInject]
        public void Construct(Action<T1> handler)
        {
            _handler = handler;
        }

        public void Execute(T1 param1)
        {
            _handler(param1);
        }
    }

    public abstract class Command<T1, T2> : ICommand
    {
        Action<T1, T2> _handler;

        [PostInject]
        public void Construct(Action<T1, T2> handler)
        {
            _handler = handler;
        }

        public void Execute(T1 param1, T2 param2)
        {
            _handler(param1, param2);
        }
    }
}
