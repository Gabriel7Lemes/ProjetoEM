﻿using EM_RepositorioAluno;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Helpers;

namespace WebApp.Controllers
{
    public class AlunoController : Controller
    {
        private readonly RepositorioAluno _repositorio;
        public AlunoController(RepositorioAluno repositorio)
        {
            _repositorio = repositorio;
        }
        public IActionResult Index(string ValorDaBusca, string TipoDeBusca)
        {
            ViewBag.Error = TempData["Error"] as string;
            if (TipoDeBusca == "Matricula")
            {
                return BuscarPorMatricula(ValorDaBusca);
            }
            if (TipoDeBusca == "Nome")
            {
                if (ValorDaBusca == null) { return RedirectToAction("Index"); }
                return BuscarPorNome(ValorDaBusca);
            }
            else
            {
                var ListaDeAlunos = ConversaoDeTipos.ConversaoListaDeDomainparaModel(_repositorio.GetAll());
                return View(ListaDeAlunos);
            }
        }

        public IActionResult BuscarPorMatricula(string valor)
        {
            if (!int.TryParse(valor, out int matricula))
            {
                ViewBag.Error = "Essa opção só aceita números";
                TempData["Error"] = ViewBag.Error;
                return RedirectToAction("Index");
            }
            var aluno = _repositorio.GetByMatricula(matricula);
            if (aluno == null)
            {
                ViewBag.Error = "Aluno não encontrado";
                TempData["Error"] = ViewBag.Error;
                return RedirectToAction("Index");
            }
            var listaDeAlunosPorMatricula = new List<AlunoModel>
            {
                ConversaoDeTipos.ConversaoDomainparaModel(aluno)
            };

            return View(listaDeAlunosPorMatricula);
        }

        public IActionResult BuscarPorNome(string valor)
        {
            var lista = _repositorio.GetByContendoNoNome(valor.ToLower());

            if (!lista.Any())
            {
                ViewBag.Error = "Aluno não encontrado";
                TempData["Error"] = ViewBag.Error;
                return RedirectToAction("Index");
            }
            else
            {
                return View(ConversaoDeTipos.ConversaoListaDeDomainparaModel(lista));
            }
        }

        public IActionResult CadastrarEditar(int id)
        {
            ViewBag.Error = TempData["Error"] as string;
            if (id == 0)
            {
                return View();
            }
            var aluno = _repositorio.GetByMatricula(id);
            if (aluno == null)
            {
                return View();
            }
            var convertido = ConversaoDeTipos.ConversaoDomainparaModel(aluno);
            return View(convertido);
        }

        [HttpPost]
        public IActionResult CadastrarEditar(AlunoModel alunoModel)
        {

            if (alunoModel.CPF != null)
            {
                if (!CpfUtils.IsCpf(alunoModel.CPF))
                {
                    ViewBag.Error = "CPF invalido";
                    TempData["Error"] = ViewBag.Error;
                    return View(alunoModel);
                }
            }
            var aluno = ConversaoDeTipos.ConversaoDeModelParaDomain(alunoModel);
            if (aluno.Matricula != 0)
            {
                _repositorio.Update(aluno);
            }
            else
            {
                _repositorio.Add(aluno);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Deletar()
        {
            return PartialView();
        }

        public IActionResult ConfirmarDeletar(int id)
        {
            var aluno = _repositorio.GetByMatricula(id);
            if (aluno != null)
            {
                _repositorio.Remove(aluno);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}