<template>
  <div class="login-container column justify-center">
    <div class="row justify-center" style="font-size: 2em; font-family: cursive; margin-bottom: 100px">
      众里寻他千百度，无件可发凭风孤
    </div>
    <div class="row justify-center">
      <embed src="resources/images/login-logo.svg" class="svg-style">

      <el-form ref="loginForm" :model="loginForm" :rules="loginRules" class="login-form" auto-complete="on"
        label-position="left">
        <el-form-item prop="userId">
          <span class="svg-container">
            <svg-icon icon-class="user" />
          </span>
          <el-input ref="userId" v-model="loginForm.userId" placeholder="userId" name="userId" type="text" tabindex="1"
            auto-complete="on" />
        </el-form-item>

        <el-form-item prop="password">
          <span class="svg-container">
            <svg-icon icon-class="password" />
          </span>
          <el-input :key="passwordType" ref="password" v-model="loginForm.password" :type="passwordType"
            placeholder="Password" name="password" tabindex="2" auto-complete="on" @keyup.enter.native="handleLogin" />
          <span class="show-pwd" @click="showPwd">
            <svg-icon :icon-class="passwordType === 'password' ? 'eye' : 'eye-open'" />
          </span>
        </el-form-item>

        <div class="row">
          <el-button :loading="signUpLoading" type="primary" class="bg-secondary" style="width: 80px;"
            @click.native.prevent="handleSignUp">
            注册
          </el-button>

          <el-button :loading="loading" type="primary" class="bg-primary col-grow" @click.native.prevent="handleLogin">
            登陆
          </el-button>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script>
// import { validuserId } from '@/utils/validate'
// import ws from '@/utils/websocket'
import signIn from './mixins/signIn.vue'

export default {
  name: 'Login',
  mixins: [signIn],
  data() {
    const validateuserId = (rule, value, callback) => {
      if (!value || value.length < 2) {
        callback(new Error('用户名长度至少 2 位'))
      } else {
        callback()
      }
    }
    const validatePassword = (rule, value, callback) => {
      if (value.length < 6) {
        callback(new Error('密码长度至少 6 位'))
      } else {
        callback()
      }
    }
    return {
      loginForm: {
        userId: 'admin',
        password: '123456'
      },
      loginRules: {
        userId: [
          { required: true, trigger: 'blur', validator: validateuserId }
        ],
        password: [
          { required: true, trigger: 'blur', validator: validatePassword }
        ]
      },
      loading: false,
      signUpLoading: false,
      passwordType: 'password',
      redirect: undefined
    }
  },
  watch: {
    $route: {
      handler(route) {
        this.redirect = route.query && route.query.redirect
      },
      immediate: true
    }
  },
  methods: {
    showPwd() {
      if (this.passwordType === 'password') {
        this.passwordType = ''
      } else {
        this.passwordType = 'password'
      }
      this.$nextTick(() => {
        this.$refs.password.focus()
      })
    }
  }
}
</script>

<style lang="scss">
/* 修复input 背景不协调 和光标变色 */
/* Detail see https://github.com/PanJiaChen/vue-element-admin/pull/927 */

$bg: #283443;
$light_gray: #283443;
$cursor: #283443;

@supports (-webkit-mask: none) and (not (cater-color: $cursor)) {
  .login-container .el-input input {
    color: $cursor;
  }
}

/* reset element-ui css */
.login-container {
  .el-input {
    display: inline-block;
    height: 47px;
    width: 85%;

    input {
      background: transparent;
      border: 0px;
      -webkit-appearance: none;
      border-radius: 0px;
      padding: 12px 5px 12px 15px;
      color: $light_gray;
      height: 47px;
      caret-color: $cursor;

      &:-webkit-autofill {
        box-shadow: 0 0 0px 1000px $bg inset !important;
        -webkit-text-fill-color: $cursor !important;
      }
    }
  }

  .el-form-item {
    border: 1px solid rgba(255, 255, 255, 0.1);
    background: rgba(0, 0, 0, 0.1);
    border-radius: 5px;
    color: #454545;
  }
}
</style>

<style lang="scss" scoped>
$bg: #2d3a4b;
$dark_gray: #889aa4;
$light_gray: #eee;

.login-container {
  min-height: 100%;
  width: 100%;
  overflow: hidden;

  .svg-style {
    height: 200px;
    margin-right: 50px;
  }

  // 设置背景图片
  // background-image: url('https://media0.giphy.com/media/opTBRh0Ydo2o15Mztf/giphy.webp?cid=ecf05e47kifiij155kqc0pp3ssgvb4wz4dc3tppjhei7j3nx&rid=giphy.webp&ct=g');
  // background-repeat: no-repeat;
  // background-size: cover;

  .login-form {
    height: 100%;
    width: 400px;
    margin-right: 70px;
    overflow: hidden;
  }

  .tips {
    font-size: 14px;
    color: #fff;
    margin-bottom: 10px;

    span {
      &:first-of-type {
        margin-right: 16px;
      }
    }
  }

  .svg-container {
    padding: 6px 5px 6px 15px;
    color: $dark_gray;
    vertical-align: middle;
    width: 30px;
    display: inline-block;
  }

  .title-container {
    position: relative;

    .title {
      font-size: 26px;
      color: $light_gray;
      margin: 0px auto 40px auto;
      text-align: center;
      font-weight: bold;
    }
  }

  .show-pwd {
    position: absolute;
    right: 10px;
    top: 7px;
    font-size: 16px;
    color: $dark_gray;
    cursor: pointer;
    user-select: none;
  }
}
</style>
