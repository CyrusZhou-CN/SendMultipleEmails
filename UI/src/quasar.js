import Vue from 'vue'

import './styles/quasar.scss'
import '@quasar/extras/material-icons/material-icons.css'
import { Quasar, Notify, Dialog } from 'quasar'

import {ChangeLanguage} from '@/utils/changeLanguage'

Vue.use(Quasar, {
  config: {
    notify: {
      position: 'top'
    }
  },
  plugins: {
    Notify,
    Dialog
  }
})
const lang = localStorage.getItem('lang') || 'zh'
ChangeLanguage(lang)
