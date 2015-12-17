using System;

namespace Zenject.Commands
{
    public interface ICommandHandlerBase
    {
    }

    public interface ICommandHandler : ICommandHandlerBase
    {
        void Execute();
    }

    public interface ICommandHandler<T1> : ICommandHandlerBase
    {
        void Execute(T1 p1);
    }

    public interface ICommandHandler<T1, T2> : ICommandHandlerBase
    {
        void Execute(T1 p1, T2 p2);
    }
}
