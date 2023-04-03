import axios from 'axios'
import { MessageBox } from 'element-ui'
import store from '@/store'
import { notifyError } from '@/components/iPrompt'

// create an axios instance
const service = axios.create({
  baseURL: process.env.VUE_APP_BASE_API, // url = base url + request url
  // withCredentials: true, // send cookies when cross-domain requests
  timeout: 5000 // request timeout
})

// request interceptor
service.interceptors.request.use(
  config => {
    // do something before request is sent

    if (store.getters.token) {
      // let each request carry token
      // ['Authorization'] is a custom headers key
      // please modify it according to the actual situation
      config.headers['Authorization'] = `Bearer ${store.getters.token}`
    }
    // console.log('request interceptor:', store.getters.token, config)
    return config
  },
  error => {
    // do something with request error
    console.log(error) // for debug
    return Promise.reject(error)
  }
)

// response interceptor
service.interceptors.response.use(
  /**
   * If you want to get http information such as headers or status
   * Please return  response => response
   */

  /**
   * Determine the request status by custom code
   * Here is just an example
   * You can also judge the status by HTTP Status Code
   */
  response => {
    const res = response.data
    // if the custom code is not 20000, it is judged as an error.
    if (res.code !== 200) {
      notifyError(res.message || 'Error')
      return Promise.reject(new Error(res.message || 'Error'))
    } else {
      return res
    }
  },
  error => {
    console.log('err:', JSON.stringify(error)) // for debug

    // 如果是 401，则跳转到登陆界面
    if (error.response.status === 401) {
      return store.dispatch('user/resetToken').then(() => {
        location.reload()
      })
    }

    notifyError(error.toString())
    return Promise.reject(error)
  }
)

export default service
