import React, { Component } from 'react';

export class TextArea extends Component {

    constructor(props) {
        super(props);
        this.state = {
            value: this.props.value
        };

        this.handleChange = this.handleChange.bind(this);
    }

    handleChange(event) {
        this.setState({ value: event.target.value });
    }

    getId() {
        return this.props.id || null;
    }

    getName() {
        return this.props.name || null;
    }

    getPlaceholder() {
        return this.props.placeholder || null;
    }

    getValue() {
        return this.props.value || "";
    }

    genElementClass() {
        return this.props.classOverride || "form-control";
    }

    isDisabled() {
        return (this.props.disabled == "true") ? true : false;
    }

    isReadOnly() {
        return (this.props.readonly == "true") ? true : false;
    }

    isRequired() {
        return (this.props.required == "true") ? true : false;
    }

    render() {
        return (<textarea id={this.getId()} className={this.genElementClass()} name={this.getName()} placeholder={this.getPlaceholder()} disabled={this.isDisabled()} readonly={this.isReadOnly()} required={this.isRequired()} value={this.state.value} onChange={this.handleChange.bind(this)}></textarea>)
    }
}

export class Input extends Component {
    constructor(props) {
        super(props);
        this.state = {
            value: this.getValue()
        };

        this.handleChange = this.handleChange.bind(this);
    }

    //static getDerivedStateFromProps(props) {
    //    return {
    //        value: props.value
    //    };
    //}

    handleChange(event) {
        this.setState({ value: event.target.value });
    }

    getId() {
        return this.props.id || null;
    }

    getName() {
        return this.props.name || null;
    }

    getPlaceholder() {
        return this.props.placeholder || null;
    }

    getType() {
        return this.props.type || null;
    }

    getValue() {
        return this.props.value || "";
    }

    genElementClass() {
        return this.props.classOverride || "form-control";
    }

    isDisabled() {
        return (this.props.disabled == "true") ? true : false;
    }

    isReadOnly() {
        return (this.props.readonly == "true") ? true : false;
    }

    isRequired() {
        return (this.props.required == "true") ? true : false;
    }

    render() {
        return (<input type={this.getType()} id={this.getId()} className={this.genElementClass()} name={this.getName()} placeholder={this.getPlaceholder()} value={this.state.value} disabled={this.isDisabled()} readonly={this.isReadOnly()} required={this.isRequired()} onChange={this.handleChange.bind(this)}/>)
    }
}