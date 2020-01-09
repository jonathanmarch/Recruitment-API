using System;
using Xunit;
using WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApi.Models;

namespace ControllerTests
{
    public class Controllers
    {
        [Fact]
        public void GetShouldReturnOKWithAllCandidates()
        {
            var controller = new RecruitmentController();
            var testData = this.GetCandidatesTestData();

            controller.SetCandidates(this.GetCandidatesTestData());

            var actionResult = controller.Get();

            // Asserts
            Assert.IsType<OkObjectResult>(actionResult);

            var okObjectResult = actionResult as OkObjectResult;
            var candidates = okObjectResult.Value as List<Candidate>;

            Assert.Equal(candidates.Count, testData.Count);
        }

        [Fact]
        public void GetShouldReturnNotFoundResult()
        {
            var controller = new RecruitmentController();
            controller.SetCandidates(this.GetCandidatesTestData());

            var actionResult = controller.Get(Guid.NewGuid()); // generate random guid that won't be found

            // Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public void GetShouldReturnOKWithCandidate()
        {
            var controller = new RecruitmentController();
            controller.SetCandidates(this.GetCandidatesTestData());

            var actionResult = controller.Get(Guid.Parse("18c46c62-3f33-4e6c-a2b2-49f7d9887051")); // try find John Smith in the List by Guid

            // Asserts
            Assert.IsType<OkObjectResult>(actionResult);

            var okObjectResult = actionResult as OkObjectResult;
            var candidate = okObjectResult.Value as Candidate;

            Assert.Equal("John", candidate.FirstName);
            Assert.Equal("Smith", candidate.LastName);
            Assert.False(candidate.ShouldSendOffer);
        }

        [Fact]
        public void PostShouldReturnCreatedWithNewCandidate()
        {
            var controller = new RecruitmentController();
            controller.SetCandidates(this.GetCandidatesTestData());

            var newCandidate = new Candidate { Id = Guid.NewGuid(), FirstName = "New", LastName = "Candidate" };
            var actionResult = controller.Post(newCandidate);

            // Assserts
            Assert.IsType<CreatedAtActionResult>(actionResult);

            var createdAtActionResult = actionResult as CreatedAtActionResult;
            var candidate = createdAtActionResult.Value as Candidate;

            Assert.Equal(newCandidate, candidate);
        }

        [Fact]
        public void PostShouldAddNewCandidateToCandidateList()
        {
            var controller = new RecruitmentController();
            controller.SetCandidates(this.GetCandidatesTestData());

            var testData = this.GetCandidatesTestData();

            Assert.Equal(RecruitmentController.candidates.Count, testData.Count);

            var newCandidate = new Candidate { Id = Guid.NewGuid(), FirstName = "New", LastName = "Candidate" };
            var actionResult = controller.Post(newCandidate);

            // Asserts
            Assert.IsType<CreatedAtActionResult>(actionResult);

            var createdAtActionResult = actionResult as CreatedAtActionResult;
            var candidate = createdAtActionResult.Value as Candidate;

            Assert.Equal(RecruitmentController.candidates.Count, testData.Count + 1);

            var findNewCandidate = RecruitmentController.candidates.Find(i => i.Id == candidate.Id);

            Assert.NotNull(findNewCandidate);
        }

        [Fact]
        public void PutShouldReturnBadRequestOnUnmatchedGuids()
        {
            var controller = new RecruitmentController();
            controller.SetCandidates(this.GetCandidatesTestData());

            var actionResult = controller.Put(Guid.NewGuid(), new Candidate { Id = Guid.NewGuid(), FirstName = "New", LastName = "Candidate" }); // Generate 2 non matching Guids

            // Asserts
            Assert.IsType<BadRequestResult>(actionResult);
        }

        [Fact]
        public void PutShouldReturnNotFound()
        {
            var controller = new RecruitmentController();
            controller.SetCandidates(this.GetCandidatesTestData());

            var invalidCandidate = new Candidate { Id = Guid.NewGuid(), FirstName = "New", LastName = "Candidate" }; // create new candidate that won't be found in exsting list
            var actionResult = controller.Put(invalidCandidate.Id, invalidCandidate);

            // Asserts
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public void PutShouldReturnNoContent()
        {
            var controller = new RecruitmentController();
            controller.SetCandidates(this.GetCandidatesTestData());

            var cloneCandidate = RecruitmentController.candidates[1];
            var updateCandidate = new Candidate { Id = cloneCandidate.Id, FirstName = cloneCandidate.FirstName, LastName = cloneCandidate.LastName };

            updateCandidate.FirstName = "Test";
            updateCandidate.ShouldSendOffer = true;

            var actionResult = controller.Put(updateCandidate.Id, updateCandidate);

            // Asserts
            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public void PutShouldUpdateCandidatesList()
        {
            var controller = new RecruitmentController();
            controller.SetCandidates(this.GetCandidatesTestData());

            var cloneCandidate = RecruitmentController.candidates[1];
            var updateCandidate = new Candidate { Id = cloneCandidate.Id, FirstName = cloneCandidate.FirstName, LastName = cloneCandidate.LastName };
            
            updateCandidate.FirstName = "Test";
            updateCandidate.ShouldSendOffer = true;

            var actionResult = controller.Put(updateCandidate.Id, updateCandidate);

            // Asserts
            Assert.Equal("Test", RecruitmentController.candidates[1].FirstName);
            Assert.True(RecruitmentController.candidates[1].ShouldSendOffer);
        }

        [Fact]
        public void DeleteShouldReturnNotFound()
        {
            var controller = new RecruitmentController();
            controller.SetCandidates(this.GetCandidatesTestData());

            var actionResult = controller.Delete(Guid.NewGuid());

            // Asserts
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public void DeleteShouldRemoveCandidateFromList()
        {
            var controller = new RecruitmentController();
            controller.SetCandidates(this.GetCandidatesTestData());

            var testData = this.GetCandidatesTestData();

            var actionResult = controller.Delete(new Guid("18c46c62-3f33-4e6c-a2b2-49f7d9887051"));

            // Asserts
            Assert.Equal(RecruitmentController.candidates.Count, testData.Count - 1);
        }
        private List<Candidate> GetCandidatesTestData()
        {
            return new List<Candidate> {
                new Candidate { Id = new Guid("18c46c62-3f33-4e6c-a2b2-49f7d9887051"), FirstName =  "John", LastName = "Smith" },
                new Candidate { Id = new Guid("75e007ca-54db-4073-8777-048b14433b9f"), FirstName =  "James", LastName = "Bennet" }
            };
        }
    }
}
