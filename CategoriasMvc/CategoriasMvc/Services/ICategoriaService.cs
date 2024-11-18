using CategoriasMvc.Models;

namespace CategoriasMvc.Services
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaViewModel>> GetCategorias();
        Task<CategoriaViewModel> GetById(int id);
        Task<CategoriaViewModel> CriaCategoria(CategoriaViewModel categoriaVM);
        Task<bool> AtualizaCategoria (int id, CategoriaViewModel categoriaVM);
        Task<bool> DeletaCategoria(int id);
    }
}
