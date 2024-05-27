// src/themes/translate.js
function translateButtonConfig(config, i18nInstance) {
  const translatedConfig = {}
  for (const key in config) {
    if (Object.prototype.hasOwnProperty.call(config, key)) {
      translatedConfig[key] = {
        ...config[key],
        label: i18nInstance.t(config[key].label),
        tooltip: i18nInstance.t(config[key].tooltip)
      }
    }
  }
  return translatedConfig
}

export default translateButtonConfig
