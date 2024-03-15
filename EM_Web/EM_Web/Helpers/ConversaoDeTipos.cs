using EM_DomainAluno;
using WebApp.Models;

namespace WebApp.Helpers
{
    public static class ConversaoDeTipos
    {
        public static AlunoModel ConversaoDomainparaModel(Aluno alunoDomain)
        {
            return new AlunoModel()
            {
                Matricula = alunoDomain.Matricula,
                Nome = alunoDomain.Nome,
                CPF = alunoDomain.CPF,
                Nascimento = alunoDomain.Nascimento,
                Sexo = (EnumeradorSexo)alunoDomain.Sexo
            };
        }

        public static IEnumerable<AlunoModel> ConversaoListaDeDomainparaModel(IEnumerable<Aluno> alunosDomain)
        {
            IEnumerable<AlunoModel> ListaConvertida = alunosDomain.Select(aluno => new AlunoModel
            {
                Matricula = aluno.Matricula,
                Nome = aluno.Nome,
                CPF = aluno.CPF,
                Nascimento = aluno.Nascimento,
                Sexo = (EnumeradorSexo)aluno.Sexo,
            });
            return ListaConvertida;
        }

        public static Aluno ConversaoDeModelParaDomain(AlunoModel alunoModel)
        {
            return new Aluno
            {
                Matricula = alunoModel.Matricula,
                Nome = alunoModel.Nome,
                Nascimento = alunoModel.Nascimento,
                CPF = alunoModel.CPF,
                Sexo = (EM_DomainEnum.EnumeradorSexo)alunoModel.Sexo
            };
        }
    }
}
