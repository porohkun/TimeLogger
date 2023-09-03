using System;
using System.Collections.Generic;

namespace TimeLogger.Attributes
{
    public class ContainerRegisterAttribute : Attribute
    {
        /// <summary>
        /// Текущая реализация будет привязана ко всем абстракциям, включая все базовые классы и интерфейсы
        /// </summary>
        public ContainerRegisterAttribute()
        {
            Abstractions = Array.Empty<Type>();
        }

        /// <summary>
        /// Текущая реализация будет привязана к выбранным абстракциям
        /// </summary>
        /// <param name="abstractions">Набор абстракций, к которым нужно привязать текущую реализацию</param>
        public ContainerRegisterAttribute(params Type[] abstractions)
        {
            Abstractions = abstractions;
        }

        /// <summary> Абстракции </summary>
        public IEnumerable<Type> Abstractions { get; }
    }
}
