namespace Cor.FileManagement.Controllers;

// The File Management document/folder/share controllers are shared by several
// menus (Company, Personal, Shared, Recent, Archive). A normal user is granted
// only the leaf permission(s) for the pages they can see, so a single-key
// [PerAuth] would 403 legitimate users. These pipe-joined sets use PerAuth's
// OR semantics: the action is allowed if the user holds ANY listed permission.
public static class FlmPerm
{
    // Reading documents (list/recent/favorites/archived/get/download).
    public const string View =
        "flm.db.view|flm.company.view|flm.company.shared.view|flm.personal.view|flm.personal.recent.view|flm.archive.view";

    // Writing documents (upload/update/delete/restore/archive/move/share-link).
    public const string Manage =
        "flm.company.manage|flm.personal.manage|flm.company.shared.upload|flm.company.shared.del|flm.company.shared.restore|flm.personal.recent.upload|flm.personal.recent.del|flm.archive.del|flm.archive.restore";

    // Reading folders.
    public const string FolderView =
        "flm.company.folders.view|flm.personal.folders.view|flm.company.view|flm.personal.view|flm.db.view";

    // Creating/updating/deleting/moving folders and folder shares.
    public const string FolderManage =
        "flm.company.folders.create|flm.company.folders.mod|flm.company.folders.del|flm.company.folders.move|flm.personal.folders.create|flm.personal.folders.mod|flm.personal.folders.del|flm.company.manage|flm.personal.manage";

    // Creating/updating a file share.
    public const string Share =
        "flm.company.shared.share|flm.personal.recent.share|flm.company.manage|flm.personal.manage";

    // Reading shares.
    public const string ShareView =
        "flm.company.shared.view|flm.personal.recent.view|flm.company.view|flm.personal.view|flm.db.view";

    // Removing a share.
    public const string ShareRemove =
        "flm.company.shared.del|flm.personal.recent.del|flm.company.manage|flm.personal.manage";
}
