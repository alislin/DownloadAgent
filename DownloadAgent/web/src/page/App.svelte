<!-- <svelte:options tag="my-app" immutable={true} /> -->
<script lang="ts">
	import { onMount } from "svelte";

	import JobApi from "../api/JobApi";
	import IconGear from "../component/IconGear.svelte";
	import type { DownloadConfig, JobInfo } from "../model/JobInfo";

	let showPath: boolean = false;
	let config: DownloadConfig = {} as DownloadConfig;
	onMount(() => {
		loadConfig();
		load();
	});

	function showConfigEdit() {
		showPath = true;
		// console.log(config);
	}

	function closeConfigEdit() {
		showPath = false;
	}

	async function loadConfig() {
		config = await JobApi.LoadConfig();
	}

	async function saveConfig() {
		showPath = false;
		await JobApi.SaveConfig(config);
	}

	function pathChange(e) {
		// console.log(e);
		config.downloadPath = e.target.value;
	}

	// export let name: string;
	let jobList: JobInfo[] = [];
	async function load() {
		jobList = await JobApi.LoadList();
		setTimeout(async () => {
			await load();
		}, 1000);
	}
</script>

<main>
	<h1>下载任务</h1>
	<div class="config">
		<div class="item">
			<span> 保存路径： </span>
			{#if showPath}
				<div class="z-1">
					<div class="z-2">
						<input class="content" type="text" bind:value={config.downloadPath} /><button on:click={saveConfig}>保存</button>
					</div>
					<div class="mask" on:click={closeConfigEdit} />
				</div>
			{:else}
				<div class="content">
					<IconGear size={21} onClick={showConfigEdit} />
					{config.downloadPath}
				</div>
			{/if}
			<!-- <input type="file" webkitdirectory on:change={pathChange} /> -->
		</div>
		<div class="item">
			<label>
				跳过已下载文件：
				<input type="checkbox" bind:checked={config.skipSameFile} on:change={saveConfig} />
			</label>
		</div>
	</div>
	<p>
		{#each jobList as job}
			<div class="row">
				<div>
					<div class="title">{job.fileName}</div>
					<div class="url">{job.url}</div>
				</div>
				<div>
					<div>
						<!-- {job.processPercentage} % -->
						<progress min="0" max={job.size} value={job.recievedByte} />
					</div>
					<div>
						{job.size}
					</div>
				</div>
			</div>
			<div class="error">{job.error}</div>
		{/each}
	</p>
</main>

<style lang="scss">
	main {
		// text-align: center;
		padding: 1em;
		max-width: 240px;
		margin: 0 auto;
	}

	h1 {
		color: #ff3e00;
		text-transform: uppercase;
		// font-size: 4em;
		font-weight: 100;
	}

	@media (min-width: 640px) {
		main {
			max-width: none;
		}
	}
	.row {
		display: flex;
		justify-content: space-between;
	}
	.url {
		white-space: pre-wrap;
		font-size: 80%;
		color: #ccc;
	}
	.title {
		white-space: pre-wrap;
	}
	.error {
		background-color: #ffece6;
		color: #ff3e00;
		padding: 3px 20px;
		margin-bottom: 10px;
		font-size: 80%;
		&:empty {
			display: none;
		}
	}
	.item {
		display: flex;
	}
	.content {
		flex: 1;
	}
	.z-2 {
		z-index: 2;
		position: absolute;
	}
	.mask {
		position: absolute;
		top: 0;
		bottom: 0;
		left: 0;
		right: 0;
		z-index: 1;
		// background-color: #ccc;
	}
</style>
