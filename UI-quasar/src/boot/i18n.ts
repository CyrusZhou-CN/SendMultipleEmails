import { boot } from 'quasar/wrappers'
import i18n from 'src/lang'

export default boot(({ app }) => {
  // 告诉应用程序使用I18n实例
  app.use(i18n)
})
