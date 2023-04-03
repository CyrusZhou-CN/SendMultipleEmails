<script>
import { notifySuccess } from '@/components/iPrompt'
import { signUp, signIn, getCurrentUserInfo } from '@/api/user'

export default {
  methods: {
    // 验证数据
    validateFormData() {
      return new Promise(resolve => {
        this.$refs.loginForm.validate(valid => {
          resolve(valid)
        })
      })
    },

    // 登录
    async handleLogin() {
      console.log('handleLogin')
      const isValid = await this.validateFormData()
      if (!isValid) {
        return
      }

      // 进行用户登陆
      const signInResult = await signIn(this.loginForm.userId, this.loginForm.password)
      if (!signInResult) return

      const { data: token } = signInResult
      // 保存 token 到 store 中
      await this.$store.dispatch('user/setToken', token)

      // 获取用户信息
      const { data: userInfo } = await getCurrentUserInfo()
      // 保存用户信息到 store 中
      await this.$store.dispatch('user/setUserInfo', userInfo)

      // 跳转到首页
      this.$router.push({ path: this.redirect || '/' })
    },

    // 注册
    async handleSignUp() {
      const isValid = await this.validateFormData()
      if (!isValid) {
        return
      }
      this.signUpLoading = true

      // 开始注册
      const signUpRes = await signUp(this.loginForm.userId, this.loginForm.password).catch(
        () => {
          this.signUpLoading = false
        }
      )
      if (!signUpRes) return

      notifySuccess('注册成功!')
      this.signUpLoading = false
    }
  }
}
</script>
