using AlunosApi.Context;
using AlunosApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AlunosApi.Services
{
    public class AlunosService : IAlunoService
    {
        private readonly AppDbContext _context;

        public AlunosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Aluno>> GetAlunos()
        {
            return await _context.Alunos.ToListAsync();
        }

        public async Task<Aluno> GetAluno(int id)
        {
            return await _context.Alunos.FindAsync(id);
        }

        public async Task<IEnumerable<Aluno>> GetAlunosByNome(string nome)
        {
            return !string.IsNullOrWhiteSpace(nome)
                ? await _context.Alunos.Where(a => a.Nome == nome).ToListAsync() 
                : await GetAlunos();
        }

        public async Task CreateAluno(Aluno aluno)
        {
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAluno(Aluno aluno)
        {
            _context.Entry(aluno).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAluno(Aluno aluno)
        {
            _context.Alunos.Remove(aluno);
            await _context.SaveChangesAsync();
        }        
    }
}
