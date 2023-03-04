function dashboardMigratorController(notificationsService, migratorResource) {
    let vm = this;
    vm.title = "title";

    vm.importDocumentTypes = importDocumentTypes;
    vm.importData = importData;
    vm.enterspeedHandle = "";
    vm.sectionId = "";

    function importDocumentTypes() {
        migratorResource.importDocumentTypes().then(function (result) {
            notificationsService.success("Document types imported");
        }, function (error) {
            notificationsService.error("Something went wrong when importing  document types: " + error);
        });
    }

    function importData() {

        migratorResource.importData(vm.enterspeedHandle, vm.sectionId).then(function (result) {
            notificationsService.success("Data imported");
        }, function (error) {
            notificationsService.error("Something went wrong when importing data: " + error);
        });
    }
}

angular.module("umbraco").controller("DashboardMigratorController", dashboardMigratorController);