using EM_DomainAluno;
using EM_DomainEnum;
using EM_RepositorioAbstrato;
using FirebirdSql.Data.FirebirdClient;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace EM_RepositorioAluno
{
    public class RepositorioAluno : RepositorioAbstrato<Aluno>
    {
        private readonly FbConnectionStringBuilder _connectionString;
        public RepositorioAluno()
        {
            FbConnectionStringBuilder connectionString = new()
            {
                UserID = "SYSDBA",
                Password = "masterkey",
                Database = @"c:\ProjetoEM\EM_Web\DBALUNOS.FB4",
                DataSource = "localhost",
                Port = 3054
            };
            _connectionString = connectionString;
        }

        public override IEnumerable<Aluno> GetAll()
        {
            using var connection = new FbConnection(_connectionString.ToString());
            var alunos = new List<Aluno>();
            try
            {
                connection.Open();
                string stringCommand = "SELECT * FROM ALUNOS ORDER BY MATRICULA;";
                var command = new FbCommand(stringCommand, connection);

                using var reader = command.ExecuteReader();
                alunos = ReadAlunos(reader);
            }
            catch (Exception ex)
            {
                ////throw new Exception("Algo deu errado: " + ex);
            }
            return alunos;
        }

        public Aluno? GetByMatricula(int matricula)
        {
            return GetAll().FirstOrDefault(alunos => alunos.Matricula == matricula); ;
        }

        public IEnumerable<Aluno> GetByContendoNoNome(string nome)
        {
            return GetAll().Where(alunos => alunos.Nome.Contains(nome));
        }

        public override IEnumerable<Aluno> Get(Expression<Func<Aluno, bool>> predicate)
        {
            return GetAll().Where(predicate.Compile());
        }

        public override void Add(Aluno aluno)
        {
            if (aluno.Nome == null) { return; }
            using var connection = new FbConnection(_connectionString.ToString());
            connection.Open();
            try
            {
                string stringCommand = "INSERT INTO ALUNOS (MATRICULA, NOME, CPF, DATANASCIMENTO, SEXO) VALUES (NEXT VALUE FOR MATRICULA_SEQ, @Nome, @CPF, @Data, @Sexo)";
                var command = new FbCommand(stringCommand, connection);

                string? CPF = aluno.CPF?.Replace(".", "").Replace("-", "");
                command.Parameters.Add("@Nome", aluno.Nome.ToString().ToLower());
                command.Parameters.Add("@CPF", !string.IsNullOrEmpty(CPF) ? CPF.ToString(): "");
                command.Parameters.Add("@Data", aluno.Nascimento.ToString("yyyy/MM/dd"));
                command.Parameters.Add("@Sexo", (int)aluno.Sexo);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ////throw new Exception("Algo deu errado: " + ex);
            }
        }
        public override void Update(Aluno aluno)
        {
            if (aluno.Matricula == 0) { return; }
            using var connection = new FbConnection(_connectionString.ToString());
            connection.Open();
            try
            {
                string stringCommand = @"UPDATE ALUNOS SET MATRICULA = @Matricula ,NOME = @Nome,CPF = @CPF, DATANASCIMENTO = @Data, SEXO = @Sexo
                                                WHERE MATRICULA = @Matricula;";
                var command = new FbCommand(stringCommand, connection);

                string? CPF = aluno.CPF?.Replace(".", "").Replace("-", "");
                command.Parameters.Add("@Matricula", aluno.Matricula);
                command.Parameters.Add("@Nome", aluno.Nome.ToLower());
                command.Parameters.Add("@CPF", CPF);
                command.Parameters.Add("@Data", aluno.Nascimento.ToString("yyyy-MM-dd"));
                command.Parameters.Add("@Sexo", (int)aluno.Sexo);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ////throw new Exception("Algo deu errado: " + ex);
            }
        }
        public override void Remove(Aluno aluno)
        {
            if (aluno == null) { return; }
            using var connection = new FbConnection(_connectionString.ToString());
            connection.Open();
            try
            {
                string stringCommand = @"DELETE FROM ALUNOS WHERE MATRICULA = @Matricula;";
                var command = new FbCommand(stringCommand, connection);

                command.Parameters.Add("@Matricula", aluno.Matricula);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ////throw new Exception("Algo deu errado: " + ex);
            }
        }


        private static List<Aluno> ReadAlunos(FbDataReader reader)
        {
            var alunos = new List<Aluno>();
            while (reader.Read())
            {
                var aluno = new Aluno
                {
                    Matricula = reader.GetInt32("MATRICULA"),
                    Nome = reader.GetString("NOME"),
                    CPF = reader.GetString("CPF"),
                    Sexo = (EnumeradorSexo)reader.GetInt32("SEXO"),
                    Nascimento = reader.GetDateTime("DATANASCIMENTO")
                };
                alunos.Add(aluno);
            }
            return alunos;
        }
    }
}