function [ dataset ] = importdataset( path )
%IMPORTDATASET Imports images and creates a featurevector

files = dir(path);
dataset = zeros(240,16384);
k=0;
for file = files'
    k=k+1;
    dataset(k,:) = reshape(imread(file.name),1,16384);
end


end

