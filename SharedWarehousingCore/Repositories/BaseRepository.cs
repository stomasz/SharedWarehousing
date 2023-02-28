using SharedWarehousingCore.DAL;
using SharedWarehousingCore.Helpers;

namespace SharedWarehousingCore.Repositories;

public class BaseRepository
{
    protected readonly MainDbContext _mainDbContext;

    public BaseRepository(MainDbContext mainDbContext)
    {
        _mainDbContext = mainDbContext;
    }
    protected async Task SaveAsync()
    {
        if (await _mainDbContext.SaveChangesAsync() <= 0)
            throw new BasicException("Unexpected Error");
    }
}