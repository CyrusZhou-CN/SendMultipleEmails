import langZH from 'quasar/lang/zh-hans.js'
import langEN from 'quasar/lang/en-us.js'
import langIT from 'quasar/lang/it.js'
import { Quasar } from 'quasar'

export const ChangeLanguage = (lang) => {
  localStorage.setItem('lang', lang)
  // 设置 Quasar 的语言
  switch (lang) {
    case 'en':
      Quasar.lang.set(langEN)
      break
    case 'zh':
      Quasar.lang.set(langZH)
      break
    case 'it':
      Quasar.lang.set(langIT)
      break
    default:
      Quasar.lang.set(langZH)
  }
}
