using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Commands
{
    /// <summary>
    /// Интерфейс диспетчеризатора, который создаёт команды на основе запроса
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Метод создания команды на основе, передаваемого запроса
        /// </summary>
        /// <param name="request">Запрос по которому создаётся команда для выполнения</param>
        /// <returns></returns>
        BaseCommand Dispatch(string request);
    }
}
