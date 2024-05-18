import { createApp } from 'vue'
import 'normalize.css/normalize.css' // A modern alternative to CSS resets

import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'

import '@/styles/index.scss' // global css

import App from './App.vue'
import store from './store'
import router from './router'

import '@/icons' // icon
import '@/permission' // permission control
import './quasar'

import AsyncComputed from 'vue-async-computed'
import AvatarCropper from 'vue-avatar-cropper'

const app = createApp(App)

// Register global components, directives, plugins etc.
app.use(AvatarCropper)
app.use(AsyncComputed)
app.use(ElementPlus, { locale: 'en' }) // Set ElementPlus lang to EN

// Vue 3 no longer has `Vue.config.productionTip`
// Vue.config.productionTip = false;

// Mount the app to the DOM
app.use(store)
app.use(router)
app.mount('#app')
