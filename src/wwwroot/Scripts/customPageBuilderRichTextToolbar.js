(function (pageBuilder) {
  var richTextEditor = (pageBuilder.richTextEditor =
    pageBuilder.richTextEditor || {});
  var configurations = (richTextEditor.configurations =
    richTextEditor.configurations || {});
  configurations["default"] = {
    toolbarVisibleWithoutSelection: true,
    paragraphFormat: {
      N: "Normal",
      H1: "Headline 1",
      H2: "Headline 2",
      H3: "Headline 3",
      H4: "Headline 4",
      H5: "Headline 5",
      H6: "Headline 6",
    },
  };
})(window.kentico.pageBuilder);
