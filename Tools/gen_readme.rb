#! /usr/bin/env ruby

require 'erb'

CUR_DIR = File.expand_path(__dir__)
SRC_DIR = File.join(CUR_DIR, '../Results')

desc_map = Dir.glob(File.join(SRC_DIR, '*.meta')).each_with_object({}) do |meta, result|
	key = File.basename(meta).gsub(/(\.meta|\.png)/, '')
	desc = `cat #{meta}`.strip
	result[key] = desc
end

template = File.read(File.join(CUR_DIR, 'readme_template.erb'))
readme = ERB.new(template).result(binding)
File.write(File.join(CUR_DIR, '../README.md'), readme)
