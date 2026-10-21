mergeInto(LibraryManager.library, {
  BrowserDemoReady: function () {
    if (typeof window.openWatersReady === 'function') window.openWatersReady();
  },
  BrowserDemoHarbor: function () {
    if (typeof window.openWatersHarborReady === 'function') window.openWatersHarborReady();
  },
  BrowserDemoFlush: function () {
    FS.syncfs(false, function (error) {
      if (typeof window.openWatersSaveFlushed === 'function') window.openWatersSaveFlushed(error ? String(error) : '');
    });
  }
});
