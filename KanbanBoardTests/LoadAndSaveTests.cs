using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using KanbanBoard.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KanbanBoard.Tests {
   [TestClass]
   public class LoadAndSaveTests {
      [TestMethod]
      public void TestCreateLoadDataItem() {
         BoardInformation createBoardInformation = BoardTestData.GetRandomFileWithData(new Random());
         BoardInformation loadBoardInformation = new BoardInformation(createBoardInformation.FilePath);
         Debug.WriteLine(createBoardInformation.FilePath);
         BoardTypeAssertions.AssertSameBoard(createBoardInformation, loadBoardInformation);
      }

      [TestMethod]
      public void TestCreateLoadEmptyItem() {
         string emptyFileLocation = Path.GetTempFileName();
         BoardInformation createBoardInformation = new BoardInformation(emptyFileLocation, new List<ColumnInformation>());
         BoardInformation loadBoardInformation = new BoardInformation(emptyFileLocation);

         BoardTypeAssertions.AssertSameBoard(createBoardInformation, loadBoardInformation);
      }

      [TestMethod]
      public void TestCreateLoadSaveLoadColumn() {
         ColumnInformation createColumnInformation = BoardTestData.GetRandomTestColumn(new Random());
         ColumnInformation loadColumnInformation = ColumnInformation.Load(createColumnInformation.ToString());
         ColumnInformation secondLoadColumnInformation = ColumnInformation.Load(createColumnInformation.ToString());

         BoardTypeAssertions.AssertColumn(loadColumnInformation, createColumnInformation);
         BoardTypeAssertions.AssertColumn(secondLoadColumnInformation, loadColumnInformation);
         BoardTypeAssertions.AssertColumn(secondLoadColumnInformation, createColumnInformation);
      }

      [TestMethod]
      public void TestCreateLoadColumn() {
         ColumnInformation createColumnInformation = BoardTestData.GetRandomTestColumn(new Random());
         ColumnInformation loadColumnInformation = ColumnInformation.Load(createColumnInformation.ToString());

         BoardTypeAssertions.AssertColumn(loadColumnInformation, createColumnInformation);
      }
   }
}
