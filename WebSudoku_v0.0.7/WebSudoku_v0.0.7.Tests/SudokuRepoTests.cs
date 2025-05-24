using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebSudoku_v0._0._7.Repositories;
using WebSudoku_v0._0._7.Models;
using Xunit;
using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Classes;

namespace WebSudoku_v0._0._7.Tests
{
    public class SudokuRepoTests
    {
        private readonly Mock<ApplicationDbContext> _mockDbContext;
        private readonly Mock<DbSet<SudokuPuzzledto>> _mockDbSet;
        private readonly Mock<ISudokuBoard> _mockSudokuBoard;
        private readonly SudokuPuzzlesRepository _repository;

        public SudokuRepoTests()
        {
            _mockDbContext = new Mock<ApplicationDbContext>();
            _mockDbSet = new Mock<DbSet<SudokuPuzzledto>>();
            _mockSudokuBoard = new Mock<ISudokuBoard>();

            _mockDbContext.Setup(x => x.Puzzle).Returns(_mockDbSet.Object);

            _repository = new SudokuPuzzlesRepository(_mockDbContext.Object, _mockSudokuBoard.Object);
        }

        [Fact]
        public async Task AddPuzzleAsync_NullPuzzle_ReturnsNull()
        {
            var result = await _repository.AddPuzzleAsync(null);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeletePuzzleAsync_NullOrEmptyPuzzle_ReturnsNull()
        {
            var result = await _repository.DeletePuzzleAsync(null);
            Assert.Null(result);

            result = await _repository.DeletePuzzleAsync(string.Empty);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPuzzleAsync_NullOrEmptyPuzzle_ReturnsNull()
        {
            var result = await _repository.GetPuzzleAsync(null);
            Assert.Null(result);

            result = await _repository.GetPuzzleAsync(string.Empty);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSolvedPuzzleAsync_NullOrEmptyPuzzle_ReturnsNull()
        {
            var result = await _repository.GetSolvedPuzzleAsync(null);
            Assert.Null(result);

            result = await _repository.GetSolvedPuzzleAsync(string.Empty);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdatePuzzleAsync_NullPuzzles_ReturnsNull()
        {
            var result = await _repository.UpdatePuzzleAsync(null);
            Assert.Null(result);
        }

        [Fact]
        public void GetEmptyListReturnModel_ReturnsListWithEmptyModel()
        {
            var result = _repository.GetEmptyListReturnModel();
            Assert.Single(result);
            Assert.Equal(Guid.Empty, result[0].Id);
            Assert.Equal(int.MinValue, result[0].Difficulty);
            Assert.Equal(string.Empty, result[0].BoardValues);
        }

        [Fact]
        public void GetEmptyReturnModel_ReturnsEmptyModel()
        {
            var result = _repository.GetEmptyReturnModel();
            Assert.Equal(Guid.Empty, result.Id);
            Assert.Equal(int.MinValue, result.Difficulty);
            Assert.Equal(string.Empty, result.BoardValues);
        }
    }
}