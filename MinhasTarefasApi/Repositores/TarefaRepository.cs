using MinhasTarefasApi.DataBase;
using MinhasTarefasApi.Models;
using MinhasTarefasApi.Repositores.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefasApi.Repositores
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly MinhasTarefasContext _banco;
        public TarefaRepository(MinhasTarefasContext banco)
        {
            _banco = banco;
        }

        public List<Tarefa> Restauracao(ApplicationUser usuario, DateTime dataUltimoSincronizacao)
        {
            var query = _banco.Tarefas
                .Where(t => t.UsuarioId == usuario.Id)
                .AsQueryable();

            if (!dataUltimoSincronizacao.Equals(null))
            {
                query.Where(t => t.Criado >= dataUltimoSincronizacao ||
                t.Atualizado >= dataUltimoSincronizacao);
            }

            return query.ToList();
        }

        /* Tarefa IdTarefaAPI - App IdTarefaAPI - Tarefa Local*/
        public List<Tarefa> Sincronizacao(List<Tarefa> tarefas)
        {
            var tarefasNovas = tarefas.Where(t => t.IdTarefaApi == 0).ToList();
            var tarefasExcluidasAtualizadas = tarefas.Where(t => t.IdTarefaApi != 0).ToList();
            // Cadastrar novos registros
            if (tarefasNovas.Count() > 0)
            {
                foreach (var tarefa in tarefasNovas)
                {
                    _banco.Tarefas.Add(tarefa);
                }
            }

            // Atualização de registro (Excluido)
            if (tarefasExcluidasAtualizadas.Count() > 0)
            {
                foreach (var tarefa in tarefasNovas)
                {
                    _banco.Tarefas.Update(tarefa);
                }
            }

            _banco.SaveChanges();

            return tarefasNovas.ToList();
        }
    }
}
