﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;
using Thomas.App.ViewModels;
using Thomas.Business.Interfaces;
using Thomas.Business.Models;
using static Thomas.App.Extensions.CustomAuthorize;

namespace Thomas.App.Controllers
{
    [Authorize]
    public class ChamadosController : BaseController
    {

        private readonly IChamadoRepository _chamadoRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMapper _mapper;
        private readonly IChamadoService _chamadoService;

        public ChamadosController(IChamadoRepository chamadoRepository,
            IFornecedorRepository fornecedorRepository,
            IMapper mapper,
            INotificador notificador,
            IChamadoService chamadoService
            ) : base(notificador)
        {
            _chamadoService = chamadoService;
            _chamadoRepository = chamadoRepository;
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [Route("lista-de-chamados")]
        public async Task<IActionResult> Index(string numeroChamado, int page = 1)
        {
            if (!string.IsNullOrEmpty(numeroChamado))
            {
                var chamado = _mapper.Map<IEnumerable<ChamadoViewModel>>(await _chamadoRepository.Buscar(c => c.NumeroChamado.Equals(numeroChamado)));
                var chamadoViewModel = PagingList.Create(chamado, 1, page);

                return View(chamadoViewModel);
            }

            var chamados = _mapper.Map<IEnumerable<ChamadoViewModel>>(await _chamadoRepository.ObterChamadosFornecedores());
            var chamadosViewModel = PagingList.Create(chamados, 10, page);

            ViewBag.numeroChamado = numeroChamado;

            chamadosViewModel.Action = "lista-de-chamados";

            return View(chamadosViewModel);
        }

        [Route("dados-do-chamado/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var chamadoViewModel = await ObterChamado(id);

            return View(chamadoViewModel);
        }

        [ClaimsAuthorize("Chamado", "Adicionar")]
        [Route("novo-chamado")]
        public async Task<IActionResult> Create()
        {

            var chamadoViewModel = await PopularFornecedores(new ChamadoViewModel());

            return View(chamadoViewModel);
        }

        [ClaimsAuthorize("Chamado", "Adicionar")]
        [Route("novo-chamado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChamadoViewModel chamadoViewModel)
        {
            if (!ModelState.IsValid) return View(chamadoViewModel);

            await _chamadoService.Adicionar(_mapper.Map<Chamado>(chamadoViewModel));

            if (!OperacaoValida()) return View(chamadoViewModel);

            return RedirectToAction("Index");
        }

        [ClaimsAuthorize("Chamado", "Editar")]
        [Route("editar-chamado/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var chamadoViewModel = await ObterChamado(id);

            if (chamadoViewModel == null)
            {
                return NotFound();
            }

            return View(chamadoViewModel);
        }

        [ClaimsAuthorize("Chamado", "Editar")]
        [Route("editar-chamado/{id:guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,  ChamadoViewModel chamadoViewModel)
        {
            if (id != chamadoViewModel.Id) return NotFound();

            if (!ModelState.IsValid) return View(chamadoViewModel);

            await _chamadoService.Atualizar(_mapper.Map<Chamado>(chamadoViewModel));

            if (!OperacaoValida()) return View(chamadoViewModel);

            return RedirectToAction("Index");
        }

        [ClaimsAuthorize("Chamado", "Excluir")]
        [Route("excluir-chamado/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var chamadoViewModel = _mapper.Map<ChamadoViewModel>(await _chamadoRepository.ObterPorId(id));

            if (chamadoViewModel == null)
            {
                return NotFound();
            }

            return View(chamadoViewModel);
        }

        [ClaimsAuthorize("Chamado", "Excluir")]
        [Route("excluir-chamado/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {

            var chamadoViewModel = _mapper.Map<ChamadoViewModel>(await _chamadoRepository.ObterPorId(id));

            if (chamadoViewModel == null) return NotFound();

            await _chamadoRepository.Remover(id);

            return RedirectToAction("Index");
        }

        private async Task<ChamadoViewModel> ObterChamado(Guid id)
        {
            var chamado = _mapper.Map<ChamadoViewModel>(await _chamadoRepository.ObterChamadoFornecedor(id));

            chamado.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

            return chamado;

        }

        private async Task<ChamadoViewModel> PopularFornecedores(ChamadoViewModel chamado)
        {
            chamado.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return chamado;
        }
    }
}
