const copydir = require('copy-dir');
const fs = require('fs')
const gulp = require('gulp')
const shell = require('gulp-shell')
const shelljs = require('shelljs')
const tar = require('tar-fs')
const waitUntil = require('wait-until')

const secretFilePath = './secret.json'

const projectName = 'BundtBot'
const projectFolder = `src/${projectName}`
const projectFileName = `project.json`
const projectFilePath = `${projectFolder}/${projectFileName}`

const buildOutputFolder = `${projectFolder}/bin/debug/netcoreapp1.1`
const publishFolder = `${buildOutputFolder}/publish`

const tarFileName = `${projectName}.tar`
const viewsFolderName = `Views`
const viewsFolder = `${projectFolder}/${viewsFolderName}`

const testFolder = 'test'
const rateLimitTestsProjectName = 'RateLimitTests'
const rateLimitTestsProjectFolder = `${testFolder}/${rateLimitTestsProjectName}`
const rateLimitTestsOutputFolder = `${rateLimitTestsProjectFolder}/bin/Debug/netcoreapp1.1`

var secret;

if (fs.existsSync(secretFilePath)) {
	secret = JSON.parse(fs.readFileSync(secretFilePath))
	// Add all the gruntfile tasks to gulp
	require('gulp-grunt')(gulp);
} else {
	gulp.stop("***Run 'node setup.js' before using gulp!***")
}

gulp.task('default', function () {
})

gulp.task('clean', function () {
	fs.unlink(tarFileName)
})

gulp.task('restore', shell.task(`dotnet restore ${projectFilePath}`, { verbose: true }))

gulp.task('dotnet-build', shell.task(`dotnet build ${projectFilePath}`, { verbose: true }))

gulp.task('copyviews', ['dotnet-build'], function () {
	copydir.sync(viewsFolder, `${buildOutputFolder}/${viewsFolderName}`);
})

gulp.task('copytokendev', ['dotnet-build'], function () {
	fs.writeFileSync(`${buildOutputFolder}/bottoken`, secret.devbottoken)
})

gulp.task('build', ['dotnet-build', 'copyviews', 'copytokendev'])

gulp.task('run', ['build', 'copyviews', 'copytokendev'], shell.task(`dotnet BundtBot.dll`, { verbose: true,  cwd: buildOutputFolder }))

gulp.task('publish', shell.task(`dotnet publish ${projectFilePath}`,
	{ verbose: true }))

gulp.task('copytokentest', ['publish'], function () {
	fs.writeFileSync(`${publishFolder}/bottoken`, secret.testbottoken)
})

gulp.task('tar', ['publish', 'copytokentest'], function (cb) {
	var pack = tar.pack(`${publishFolder}`)
		.pipe(fs.createWriteStream(tarFileName))
	waitUntil()
		.interval(1000)
		.times(50)
		.condition(function () {
			console.log('bytes written: ' + pack.bytesWritten)
			return pack._writableState.ended
		})
		.done(function (result) {
			cb()
		})
})

gulp.task('sftpdeploy', ['tar'], shell.task('grunt sftp:deploy', { verbose: true, }))

gulp.task('sshdeploy', ['sftpdeploy'], shell.task('grunt sshexec:deploy', { verbose: true, }))

gulp.task('deploy', ['publish', 'tar', 'sftpdeploy', 'sshdeploy'])

// Start test commands

gulp.task('test', shell.task('dotnet test test/BundtBotTests/project.json',
	{ verbose: true }))

gulp.task('integration-tests', () => shelljs.exec('dotnet test test/IntegrationTests/project.json'))

gulp.task('rate-limiter-tests', () => shelljs.exec(`dotnet test ${rateLimitTestsProjectFolder}/project.json`))

// Start remote server commands

gulp.task('rlogs', shell.task(
	`ssh ${secret.testusername}@${secret.testhost} "journalctl -f -o cat -u bundtbot.service"`,
	{ verbose: true }))

gulp.task('setup-server', shell.task('grunt sshexec:setup', { verbose: true, }))
