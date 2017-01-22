using System;

namespace Zenject
{
    [ZenjectAllowDuringValidationAttribute]
    public class Lazy<T>
    {
        readonly DiContainer _container;
        readonly InjectContext _context;

        bool _hasValue;
        T _value;

        public Lazy(DiContainer container, InjectContext context)
        {
            _container = container;
            _context = context;

            if (container.IsValidating)
            {
                var value = this.Value;
            }
        }

        public T Value
        {
            get
            {
                if (!_hasValue)
                {
                    _value = _container.Resolve<T>(_context);
                    _hasValue = true;
                }

                return _value;
            }
        }
    }
}
