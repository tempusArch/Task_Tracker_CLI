using tracker.Models;

namespace tracker.Interfaces {
    public interface INinmuServices {
        Task<int> addNinmu(string m);
        Task<bool> updateNinmu(int i, string m);
        Task<bool> deleteNinmu(int i);
        Task<bool> statusSetti(int i, string m);
        Task<List<Ninmu>> getAllNinmu();
        Task<List<Ninmu>> getNinmuByStatus(string m);
        List<string> getAllCommands();

    }
}