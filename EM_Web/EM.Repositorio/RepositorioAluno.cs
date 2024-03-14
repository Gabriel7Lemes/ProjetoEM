﻿using EM_DomainAluno;
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
                //Database = "C:\\Users\\Escolar Manager\\Desktop\\Projeto_EM\\DBALUNOS.FB4",
                Database = "D:\\ProjetoEM\\EM_Web\\DBALUNOS.FB4",
                //DataSource = "192.168.1.143",
                DataSource = "localhost",
                //Port = 3054
                //Database = "C:\\Users\\gabri\\OneDrive\\Área de Trabalho\\Projeto_EM\\DBALUNOS.FB4",
                //DataSource = "localhost",
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
                string stringCommand = "SELECT * FROM TBALUNO ORDER BY ALUMATRICULA;";
                var command = new FbCommand(stringCommand, connection);

                using var reader = command.ExecuteReader();
                alunos = ReadAlunos(reader);
            }
            catch (Exception ex)
            {
                //throw new Exception("Algo deu errado: " + ex);
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
                string stringCommand = "INSERT INTO TBALUNO (ALUMATRICULA, ALUNOME, ALUCPF, ALUNASCIMENTO, ALUSEXO) VALUES((GEN_ID(GEN_TBALUNO, 1)), @Nome, @CPF, @Data, @Sexo);";
                var command = new FbCommand(stringCommand, connection);

                string? CPF = aluno.CPF?.Replace(".", "").Replace("-", "");
                command.Parameters.Add("@Nome", aluno.Nome.ToLower());
                command.Parameters.Add("@CPF", CPF);
                command.Parameters.Add("@Data", aluno.Nascimento.ToString("yyyy/MM/dd"));
                command.Parameters.Add("@Sexo", (int)aluno.Sexo);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Algo deu errado: " + ex);
            }
        }
        public override void Update(Aluno aluno)
        {
            if (aluno.Matricula == 0) { return; }
            using var connection = new FbConnection(_connectionString.ToString());
            connection.Open();
            try
            {
                string stringCommand = @"UPDATE TBALUNO SET ALUMATRICULA = @Matricula ,ALUNOME = @Nome,ALUCPF = @CPF, ALUNASCIMENTO = @Data, ALUSEXO = @Sexo
                                                WHERE ALUMATRICULA = @Matricula;";
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
                throw new Exception("Algo deu errado: " + ex);
            }
        }
        public override void Remove(Aluno aluno)
        {
            if (aluno == null) { return; }
            using var connection = new FbConnection(_connectionString.ToString());
            connection.Open();
            try
            {
                string stringCommand = @"DELETE FROM TBALUNO  WHERE ALUMATRICULA = @Matricula;";
                var command = new FbCommand(stringCommand, connection);

                command.Parameters.Add("@Matricula", aluno.Matricula);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Algo deu errado: " + ex);
            }
        }


        private static List<Aluno> ReadAlunos(FbDataReader reader)
        {
            var alunos = new List<Aluno>();
            while (reader.Read())
            {
                var aluno = new Aluno
                {
                    Matricula = reader.GetInt32("ALUMATRICULA"),
                    Nome = reader.GetString("ALUNOME"),
                    CPF = reader.GetString("ALUCPF"),
                    Sexo = (EnumeradorSexo)reader.GetInt32("ALUSEXO"),
                    Nascimento = reader.GetDateTime("ALUNASCIMENTO")
                };
                alunos.Add(aluno);
            }
            return alunos;
        }
    }
}