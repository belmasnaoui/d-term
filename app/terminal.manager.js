const repository = require('./repository');
const GoldenLayout = require('golden-layout');
const { remote, ipcRenderer } = require('electron');
const terminal = require('./terminal.instance');
const newId = require('uuid/v1');
const _ = require('lodash');

const config = {
  settings: {
    showPopoutIcon: false,
    showMaximiseIcon: true,
    showCloseIcon: true
  },
  content: []
};

let terminals = [];
let latestActiveTerminal = null;

let layout = new GoldenLayout(config, document.getElementById('terminals'));

layout.registerComponent('terminal', function (container, descriptor) {

  let element = container.getElement();

  container._config.id = newId();
  element.css({
    'background-color': descriptor.xterm.theme.background
  });

  let wrapper = $('<div/>', {
    id: container._config.id,
    class: 'terminal-wrapper'
  }).appendTo(element);

  let loader = $('<div/>', {
    class: 'standby'
  }).appendTo(element);

  setTimeout(() => {
    let instance = new terminal(container._config);

    instance.on('xterm-focused', (info) => {
      let terminal = _.find(terminals, terminal => terminal.id == info.id);
      if (terminal) {
        latestActiveTerminal = terminal;
      }
    });

    instance.on('node-pty-exited', (info) => {
      ipcRenderer.send('info', { event: 'node-pty-exited', info });
      let terminal = _.find(terminals, terminal => terminal.id == info.id);
      let index = terminals.indexOf(terminal);
      terminals.splice(index, 1);
      if (!terminal.destroyed) {
        container.close();
      }
    });

    container.on('destroy', () => {
      let config = container._config;
      let terminal = _.find(terminals, terminal => terminal.id == config.id);
      terminal.destroy();
    });

    container.on('resize', () => {
      let config = container._config;
      let terminal = _.find(terminals, terminal => terminal.id == config.id);
      terminal.resize();
    });

    terminals.push(instance);
    setFocus(instance);
    loader.remove();

  }, 1000);

});

layout.on('initialised', function () {
  createPrimaryInstance();
});

layout.on('stateChanged', function () {
  resizeAllTerminals();
});

function setFocus(instance) {
  if (instance) {
    instance.setFocus();
  }
}

function resizeAllTerminals() {
  terminals.forEach(terminal => {
    terminal.resize();
  });
}

function getConsoleOptionConfig(consoleOption) {
  var newItemConfig = {
    type: 'component',
    title: consoleOption.label,
    componentName: 'terminal',
    componentState: {
      consoleOption,
      nodePty: {
        name: 'xterm-color'
      },
      xterm: {
        cursorBlink: true,
        cursorStyle: 'block',
        rightClickSelectsWord: true,
        theme: { background: '#0a0a0a' }
      }
    }
  };
  return newItemConfig;
}

function createTerminalInstance(consoleOption) {
  var newItemConfig = getConsoleOptionConfig(consoleOption);
  if (layout.root.contentItems && layout.root.contentItems.length) {
    layout.root.contentItems[0].addChild(newItemConfig);
    return;
  }
  layout.root.addChild(newItemConfig);
}

function getPrimaryOption() {
  let options = repository.getConsoleOptions();
  let primary = _.filter(options, option => option.primary);
  if (primary && primary.length) {
    return primary[0];
  }
  return options[0];
}

function createPrimaryInstance() {
  let option = getPrimaryOption();
  createTerminalInstance(option);
}

function setDraggableOptions() {
  $('a.console-option-action').each(function (idx, item) {
    let $this = $(item);
    let consoleId = $this.data('option-id');
    let consoleOption = repository.getById(consoleId);
    layout.createDragSource($this, getConsoleOptionConfig(consoleOption));
  });
}

exports.create = (consoleOption) => {
  createTerminalInstance(consoleOption);
}

exports.terminateAll = () => {
  terminals.forEach(instance => {
    instance.terminate();
  });
}

exports.focusLastInstance = () => {
  if (latestActiveTerminal == null && terminals.length) {
    latestActiveTerminal = terminals[0];
  }
  setFocus(latestActiveTerminal);
}

exports.updateSize = () => {
  layout.updateSize();
}

exports.initialize = () => {
  layout.init();
  setDraggableOptions();
}