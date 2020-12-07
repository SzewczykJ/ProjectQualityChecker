using System.Collections.Generic;
using ProjectQualityChecker.Data.Database;

namespace ProjectQualityChecker.Data.IDataRepository
{
    public interface ILanguageRepo
    {
        Language FindByName(string name);
        List<Language> GetAll();
    }
}