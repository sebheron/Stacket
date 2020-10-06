using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using KanbanBoard.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KanbanBoard.Tests
{
    [TestClass]
    public class LoadAndSaveTests
    {
        [TestMethod]
        public void TestCreateLoadDataItem()
        {
            var createBoardInformation = BoardTestData.GetRandomFileWithData(new Random());
            var loadBoardInformation = new BoardInformation(createBoardInformation.FilePath);
            Debug.WriteLine(createBoardInformation.FilePath);
            BoardTypeAssertions.AssertSameBoard(createBoardInformation, loadBoardInformation);
        }

        [TestMethod]
        public void TestCreateLoadEmptyItem()
        {
            var emptyFileLocation = Path.GetTempFileName();
            var createBoardInformation = new BoardInformation(emptyFileLocation, new List<ColumnInformation>());
            var loadBoardInformation = new BoardInformation(emptyFileLocation);

            BoardTypeAssertions.AssertSameBoard(createBoardInformation, loadBoardInformation);
        }

        [TestMethod]
        public void TestCreateLoadSaveLoadColumn()
        {
            var createColumnInformation = BoardTestData.GetRandomTestColumn(new Random());
            var loadColumnInformation = ColumnInformation.Load(createColumnInformation.ToString());
            var secondLoadColumnInformation = ColumnInformation.Load(createColumnInformation.ToString());

            BoardTypeAssertions.AssertColumn(loadColumnInformation, createColumnInformation);
            BoardTypeAssertions.AssertColumn(secondLoadColumnInformation, loadColumnInformation);
            BoardTypeAssertions.AssertColumn(secondLoadColumnInformation, createColumnInformation);
        }

        [TestMethod]
        public void TestCreateLoadColumn()
        {
            var createColumnInformation = BoardTestData.GetRandomTestColumn(new Random());
            var loadColumnInformation = ColumnInformation.Load(createColumnInformation.ToString());

            BoardTypeAssertions.AssertColumn(loadColumnInformation, createColumnInformation);
        }
    }
}